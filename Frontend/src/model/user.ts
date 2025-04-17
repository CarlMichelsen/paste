export type PublicUser = {
    id: string;
    authenticationMethod: string;
    username: string;
    avatarUrl: string;
}

export type AuthenticatedUser = {
    authenticationId: string;
    email: string;
} & PublicUser