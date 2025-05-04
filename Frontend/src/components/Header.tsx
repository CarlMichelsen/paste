import React from "react";
import {AuthenticatedUser} from "../model/user.ts";

type HeaderProps = {
    user: AuthenticatedUser|null;
}

const Header: React.FC<HeaderProps> = ({ user }) => {

    return (
        <header>
            <p>Header</p>

            {user?.username}
        </ header>
    );
}

export default Header;