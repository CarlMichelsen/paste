import React from "react";
import {AuthenticatedUser} from "../model/user.ts";
import ProfileCard from "./ProfileCard.tsx";
import {UserClient} from "../util/clients/userClient.ts";

type HeaderProps = {
    user: AuthenticatedUser|null;
}

const Header: React.FC<HeaderProps> = ({ user }) => {
    const login = () => {
        const userClient = new UserClient();
        window.location.replace(userClient.getLoginUrl());
    }

    return (
        <header className="grid grid-cols-[1fr_12rem] h-12 sm:mt-4 mb-2 sm:mb-6">
            <h1 className="text-4xl italic font-mono">Paste</h1>

            {user
                ? <ProfileCard user={user} />
                : <button className="hover:underline cursor-pointer" onClick={login}>Login</button>}
        </ header>
    );
}

export default Header;