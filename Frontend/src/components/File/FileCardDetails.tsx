import {FC} from "react";
import {FileDto} from "../../model/file/fileDto.ts";
import {FileClient} from "../../util/clients/fileClient.ts";
import {useQueryClient} from "@tanstack/react-query";

type FileCardDetailsProps = {
    file: FileDto;
}

const formatFileSize = (bytes: number): string => {
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB'];

    if (bytes === 0) return '0 B';

    const order = Math.floor(Math.log(bytes) / Math.log(1024));
    const value = bytes / Math.pow(1024, order);

    return `${value.toFixed(2).replace(/\.0+$|(\.\d*[1-9])0+$/, '$1')} ${sizes[order]}`;
}

const formatDate = (dateString: string): string =>
{
    const date = new Date(dateString);
    const dateFormatter = new Intl.DateTimeFormat(navigator.language, {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
    const timeFormatter = new Intl.DateTimeFormat(navigator.language, {
        hour: 'numeric',
        minute: 'numeric',
        second: 'numeric',
        hour12: false
    });
    
    return `${dateFormatter.format(date)}, ${timeFormatter.format(date)}`;
}    

const FileCardDetails: FC<FileCardDetailsProps> = ({ file }) => {
    const queryClient = useQueryClient();
    
    const downloadFile = async (file: FileDto) => {
        const client = new FileClient();
        const path = client.downloadEndpoint + "/" + file.id;
        fetch(path, { credentials: "include" })
            .then(response => response.blob())
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = file.name;
                a.click();
                window.URL.revokeObjectURL(url);
            });
    }
    
    return (
        <div className="grid grid-cols-[auto_10rem]">
            <div className="grid grid-cols-2 pl-1">
                <div>Uploaded {formatDate(file.createdAt)}</div>
                <div>{file.mimeType}</div>
                <div>{formatFileSize(file.size)}</div>
                <div>
                    <p className="text-xs pt-1">{file.id}</p>
                </div>
            </div>
            <div>
                <button
                    onClick={async () => {
                        await downloadFile(file);
                    }}
                    className="cursor-pointer hover:underline">Download</button>
                <br/>
                <button
                    onClick={async () =>
                    {
                        if (confirm(`Delete file '${file.name}' ${formatFileSize(file.size)}`)) {
                            const fileClient = new FileClient();
                            const deleted = await fileClient.deleteFile(file.id)
                            if (deleted) {
                                await queryClient.refetchQueries({ queryKey: ["files"], exact: false})
                            }
                        }
                    }}
                    className="cursor-pointer hover:underline">Delete</button>
            </div>
        </div>
    );
}

export default FileCardDetails;