import {FC, useState} from "react";
import {FileClient} from "../util/clients/fileClient.ts";
import {useQuery, useQueryClient} from "@tanstack/react-query";
import FileCard from "./File/FileCard.tsx";

type FileExplorerProps = {
    maxFileAmount: number;
}

const FileExplorer: FC<FileExplorerProps> = ({ maxFileAmount }) => {
    const queryClient = useQueryClient();
    const [search, setSearch] = useState<string|null>(null);
    const client = new FileClient();
    
    const safelyGetRecentFiles = async (skip: number, amount: number) => {
        const res = await client.getFiles(skip, amount)
        if (!res.ok) {
            throw new Error(res.errors.join('\n'));
        }
        return res.value!;
    }

    const safelySearchFiles = async (filenameSearchQuery: string, amount: number) => {
        const res = await client.searchFiles(filenameSearchQuery, amount)
        if (!res.ok) {
            throw new Error(res.errors.join('\n'));
        }
        return res.value!;
    }

    const { data, isLoading } = useQuery({
        queryKey: ['files'],
        queryFn: () => search === null ? safelyGetRecentFiles(0, maxFileAmount) : safelySearchFiles(search, maxFileAmount),
        staleTime: 4 * 60 * 1000, // 4 minutes in milliseconds
    });
    
    const handleSetSearch = async (val: string) => {
        let newValue: string|null = val;
        if (val == null)
        {
            // doublequotes could mean undefined too.
            newValue = null;
        }
        
        if (val.length == 0)
        {
            newValue = null;
        }
        
        setSearch(newValue);
        setTimeout(() => queryClient.refetchQueries({ queryKey: ["files"], exact: false}), 50);
    }
    
    return (
        <div>
            <div className="grid grid-cols-[auto_1fr] gap-2 pr-2 mb-2">
                <h3 className="text-2xl italic mb-2">Find file</h3>
                <input
                    onChange={(e) => handleSetSearch(e.target.value)}
                    type="search"
                    className="text-lg transition-colors h-10 px-2 rounded-sm focus:bg-neutral-300 bg-neutral-200 dark:focus:bg-neutral-600 dark:bg-neutral-800 focus:outline-none" />
            </div>
            {isLoading ? (
                <p>Loading...</p>
            ) : (
                <ol className="border-l pl-2 space-y-2 mr-2 h-96 overflow-y-scroll">
                    {data?.map(file => (
                        <li key={file.id} className="block">
                            <FileCard file={file}/>
                        </li>
                    ))}
                </ol>
            )}
        </div>
    );
}

export default FileExplorer