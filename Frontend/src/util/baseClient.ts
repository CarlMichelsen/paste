import { v4 as uuidV4 } from 'uuid';
import { errorResponse, type ServiceResponse } from "../model/serviceResponse";
import { UserAccessor } from './userAccessor';

export type Method = "GET"
    | "HEAD"
    | "POST"
    | "PUT"
    | "DELETE"
    | "CONNECT"
    | "OPTIONS"
    | "TRACE"
    | "PATCH";

export abstract class BaseClient
{
    protected abstract host: string;
    
    protected async requestStream<T>(
        method: Method,
        path: string,
        handler: (chunk: T) => Promise<void>,
        body?: object,
        headers?: { [key: string]: string },
        traceId?: string)
    {
        try {
            return await this.internalStreamRequest<T>(method, path, handler, body, headers, traceId);
        } catch (error) {
            const userRes = await UserAccessor.getUser();
            if (userRes.ok) {
                // User is logged in so this must be a genuine error.
                throw error;
            }

            const refreshRes = await UserAccessor.refresh();
            if (!refreshRes.ok) {
                throw error;
            }
        }

        try {
            return await this.internalStreamRequest<T>(method, path, handler, body, headers, traceId);
        } catch (error) {
            throw error;
        }
    }

    protected async request<T>(
        method: Method,
        path: string,
        body?: object,
        headers?: { [key: string]: string },
        traceId?: string)
    {
        try {
            return await this.internalRequest<T>(method, path, body, headers, traceId);
        } catch (error) {
            const userRes = await UserAccessor.getUser();
            if (userRes.ok) {
                // User is logged in so this must be a genuine error.
                return errorResponse<T>();
            }

            const refreshRes = await UserAccessor.refresh();
            if (!refreshRes.ok) {
                return errorResponse<T>();
            }
        }

        try {
            return await this.internalRequest<T>(method, path, body, headers, traceId);
        } catch (error) {
            return errorResponse<T>();
        }
    }
    
    private static async readLine(
        reader:  ReadableStreamDefaultReader<Uint8Array>,
        decoder: TextDecoder)
    {
        return new ReadableStream<string>({
            start(controller) {
                let buffer = "";

                function pump(): Promise<void> {
                    return reader.read().then(({ done, value }) => {
                        if (done) {
                            if (buffer) {
                                controller.enqueue(buffer); // Emit any remaining buffered line
                            }
                            controller.close(); // Close the stream
                            return;
                        }

                        // Decode the chunk and append to the buffer
                        buffer += decoder.decode(value, { stream: true });

                        // Split the buffer by newlines to process each line
                        let lines = buffer.split('\n');

                        // Enqueue all lines except the last one (which may be incomplete)
                        for (let i = 0; i < lines.length - 1; i++) {
                            controller.enqueue(lines[i]);
                        }

                        // Retain the last unprocessed part of the buffer
                        buffer = lines[lines.length - 1];

                        // Keep pumping
                        return pump();
                    }).catch(error => {
                        console.error("Stream reading error:", error);
                        controller.error(error); // Signal an error
                    });
                }

                pump();
            }
        });
    }
    
    private async internalStreamRequest<T>(
        method: Method,
        path: string,
        handler: (chunk: T) => Promise<void>,
        body?: object,
        headers?: { [key: string]: string },
        traceId?: string)
    {
        const fullDestination = this.host + this.ensureLeadingSlash(path);
        console.log("waiting for response")
        const response = await this.internalFetch(method, fullDestination, body, headers, traceId);
        if (!response.body) {
            throw new Error(`${method} "${fullDestination}" response did not have a body`);
        }

        console.log("waiting for stream");
        const stream = await BaseClient.readLine(
            response.body.getReader(),
            new TextDecoder("utf-8"));
        console.log("received stream");
        const reader = stream.getReader();
        
        while (true) {
            const { done, value } = await reader.read();
            if (done) {
                break;
            }
            
            if (!value) {
                continue;
            }
            
            const object = JSON.parse(value) as T;
            await handler(object);
        }
    }

    private async internalRequest<T>(
        method: Method,
        path: string,
        body?: object,
        headers?: { [key: string]: string },
        traceId?: string): Promise<ServiceResponse<T>>
    {
        const fullDestination = this.host + this.ensureLeadingSlash(path);
        const response = await this.internalFetch(method, fullDestination, body, headers, traceId);

        if (response.ok) {
            return (await response.json()) as ServiceResponse<T>;
        } else {
            throw new Error(`${method} "${fullDestination}" request failed with status: ${response.status}`);
        }
    }
    
    private async internalFetch(
        method: Method,
        fullDestination: string,
        body?: object,
        headers?: { [key: string]: string },
        traceId?: string): Promise<Response>
    {
        const actualTraceId = traceId ?? uuidV4();
        const init: RequestInit = {
            method: method,
            credentials: 'include',
            headers: {
                "Content-Type": "application/json",
                "X-Trace-Id": actualTraceId,
                ...headers },
        };

        if (body) {
            init.body = JSON.stringify(body);
        }
        
        return await fetch(fullDestination, init);
    }

    private ensureLeadingSlash(input: string): string {
        if (!input.startsWith("/")) {
            return "/" + input;
        }

        return input;
    }
}