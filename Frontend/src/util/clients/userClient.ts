import { AuthenticatedUser } from "../../model/user";
import { BaseClient } from "../baseClient";
import { identityUrl } from "../endpoints";

export class UserClient extends BaseClient
{
    public readonly userPath: string = "api/v1/User";

    protected host: string = identityUrl();

    public async getUser() {
        return await this.request<AuthenticatedUser>("GET", this.userPath);
    }
    public async logout() {
        return await this.request<void>("DELETE", this.userPath);
    }

    public getLoginUrl(): string {
        const provider = import.meta.env.VITE_APP_ENV === 'development'
            ? "development"
            : "discord";
        return `${this.host}/api/v1/login/${provider}?dest=${location.href}`;
    }
}