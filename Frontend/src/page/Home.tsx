import React from "react";
import {FileClient} from "../util/clients/fileClient.ts";
import FileExplorer from "../components/FileExplorer.tsx";
import FileUploader from "../components/File/FileUploader.tsx";
import {useQueryClient} from "@tanstack/react-query";
import {applicationStore} from "../store/applicationStore.ts";

const Home: React.FC = () => {
    const client = new FileClient();
    const queryClient = useQueryClient()
    const appStore = applicationStore();
    
    return appStore.user ? (
        <>
            <FileUploader
                endpoint={client.uploadEndpoint}
                onProgress={progress => console.log(progress)}
                onUploaded={async () => await queryClient.refetchQueries({ queryKey: ["files"], exact: false})} />
            
            <FileExplorer maxFileAmount={8} />
        </>
    ) : (
        <p>Login required</p>
    );
}

export default Home;