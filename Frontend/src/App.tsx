import React, {ReactNode, useEffect} from "react";
import Header from "./components/Header";
import {useQuery} from "@tanstack/react-query";
import {applicationStore} from "./store/applicationStore.ts";
import {getUser} from "./util/getUser.ts";

interface AppProps {
    children?: ReactNode;
}

const App: React.FC<AppProps> = ({ children }) => {
    const appStore = applicationStore();
    
    const safelyGetUser = async () => {
        const res = await getUser();
        if (!res.ok || !res.value)
        {
            throw new Error(res.errors.join("\n"));
        }
        
        return res.value;
    }
    
    const { data, isLoading } = useQuery({
        queryKey: ['user'],
        queryFn: () => safelyGetUser(),
        enabled: !appStore.user,
        retry: false,
    });
    
    useEffect(() => appStore.setUser(data ?? null), [isLoading, data])
    
    return isLoading ? (
            <p className="container mx-auto relative z-10">Loading...</p>
        ) : (
            <main className="container mx-auto grid relative z-10">
                <Header user={appStore.user}/>
                {children}
            </main>
        );
}

export default App;