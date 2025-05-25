import {BaseClient} from "../baseClient";
import {hostUrl} from "../endpoints";
import {FileDto} from "../../model/file/fileDto.ts";

export class FileClient extends BaseClient {
    protected host: string = hostUrl();
    
    public static readonly filePath: string = "api/v1/file";
    public readonly uploadEndpoint: string = `${this.host}/${FileClient.filePath}/upload`;
    public readonly downloadEndpoint: string = `${this.host}/${FileClient.filePath}/download`;

    public async getFiles(skip: number, amount: number) {
        return await this.request<FileDto[]>("GET", `${FileClient.filePath}/list/${skip}/${amount}`);
    }

    public async searchFiles(partialFileName: string, amount: number) {
        return await this.request<FileDto[]>("GET", `${FileClient.filePath}/search/${partialFileName}/${amount}`);
    }

    public async getFile(fileId: string) {
        return await this.request<FileDto>("GET", `${FileClient.filePath}/${fileId}`);
    }

    public async deleteFile(fileId: string) {
        return await this.request<boolean>("DELETE", `${FileClient.filePath}/${fileId}`);
    }
    
    public async getAntiforgeryToken() {
        return await this.request<string>("GET", `${FileClient.filePath}/antiforgery/token`);
    }
}