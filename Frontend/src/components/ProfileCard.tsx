import React from "react";
import {AuthenticatedUser} from "../model/user";
import {UserClient} from "../util/clients/userClient.ts";
import {useQueryClient} from "@tanstack/react-query";
import {applicationStore} from "../store/applicationStore.ts";

type ProfileCardProps = {
    user: AuthenticatedUser
}

const ProfileCard: React.FC<ProfileCardProps> = ({ user }) => {
    const appStore = applicationStore();
    const queryClient = useQueryClient()
    
    const logout = async () => {
        const userClient = new UserClient();
        const logoutRes = await userClient.logout();
        if (logoutRes.ok) {
            await queryClient.invalidateQueries({ queryKey: ['auth'] });
            appStore.setUser(null)
        }
    }
    
    return (
        <div className="grid grid-rows-1 grid-cols-[max-content_1fr]">
            <img
                className="aspect-square w-12 rounded-sm"
                src={user.avatarUrl}
                alt="profile"/>
            <div>
                <h3>{user.username}</h3>
                <button className="hover:underline text-xs cursor-pointer" onClick={() => logout()}>Logout</button>
            </div>
        </div>
    );
}

export default ProfileCard;