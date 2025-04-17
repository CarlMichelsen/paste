import { errorResponse, type ServiceResponse } from "../model/serviceResponse";
import type { AuthenticatedUser } from "../model/user";
import { identityUrl } from "./endpoints";
import { v4 as uuidV4 } from 'uuid';

export class UserAccessor
{
    public static async getUser(): Promise<ServiceResponse<AuthenticatedUser>>
    {
        try {
            const response = await fetch(
                `${this.host()}/api/v1/User`,
                this.getInit("GET"));

            if (!response.ok) {
                return errorResponse();
            }

            return (await response.json()) as ServiceResponse<AuthenticatedUser>;
        } catch (error) {
            console.error('UserAccessor.getUser', error);
            return errorResponse();
        }
    }

    public static async refresh(): Promise<ServiceResponse<void>>
    {
        try {
            const response = await fetch(
                `${this.host()}/api/v1/User`,
                this.getInit("PUT"));

            if (!response.ok) {
                return errorResponse();
            }

            return (await response.json()) as ServiceResponse<void>;
        } catch (error) {
            console.error('UserAccessor.refresh', error);
            return errorResponse();
        }
    }

    private static getInit(method: string, traceId?: string) : RequestInit
    {
        return {
            method: method,
            credentials: 'include',
            headers: {
                "Content-Type": "application/json",
                "X-Trace-Id": traceId ?? uuidV4(),
            },
        };
    }

    private static host()
    {
        return identityUrl();
    }
}