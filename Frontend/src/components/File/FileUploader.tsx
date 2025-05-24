import {CSSProperties, FC, useState} from "react";
import { v4 as uuidV4 } from 'uuid';
import FileSelector from "./FileSelector.tsx";
import {FileClient} from "../../util/clients/fileClient.ts";

type FileUploaderProps = {
    endpoint: string;
    onProgress: (progress: number) => void;
    onUploaded: () => void;
}

const FileUploader: FC<FileUploaderProps> = ({ endpoint, onProgress, onUploaded }) => {
    const client = new FileClient();
    
    const [error, setError] = useState<string | null>(null);
    const [isUploading, setIsUploading] = useState<boolean>(false);
    const [uploadProgress, setUploadProgress] = useState<number>(0);
    const [file, setFile] = useState<File|null>(null);
    
    const onFileSelect = (file: File) => {
        setError(null)
        setFile(file)
    }

    const onFileReset = () => {
        setError(null)
        setFile(null)
    }
    
    const resetUpload = () => {
        setFile(null)
        setIsUploading(false)
    }
    
    const uploadFile = async (file: File, antiforgeryToken: string) => {
        return new Promise<void>((resolve, reject) => {
            setError(null);
            setIsUploading(true);
            setUploadProgress(0);

            const formData = new FormData();
            formData.append('formFile', file);

            try {
                const xhr = new XMLHttpRequest();
                xhr.upload.addEventListener('progress', (event) => {
                    if (event.lengthComputable) {
                        const percentComplete = Math.round((event.loaded / event.total) * 100);
                        onProgress(percentComplete);
                    }
                });
                
                xhr.open('POST', endpoint, true);
                xhr.withCredentials = true;
                xhr.setRequestHeader("X-Trace-Id", uuidV4());
                xhr.setRequestHeader("X-XSRF-TOKEN", antiforgeryToken);

                xhr.onload = () => {
                    if (xhr.status >= 200 && xhr.status < 300) {
                        try {
                            resetUpload()
                            onUploaded();
                            resolve();
                        } catch (e) {
                            resetUpload()
                            const errorText = "Invalid JSON response from server"
                            setError(errorText);
                            reject(new Error(errorText));
                        }
                    } else {
                        try {
                            const errorData = JSON.parse(xhr.responseText);
                            reject(new Error(errorData.message || `Upload failed with status: ${xhr.status}`));
                        } catch (e) {
                            resetUpload()
                            const errorText = `Upload failed with status: ${xhr.status}`
                            setError(errorText);
                            reject(new Error(errorText));
                        }
                    }
                };

                xhr.onerror = () => {
                    resetUpload()
                    reject(new Error('Network error occurred during upload'));
                };

                xhr.ontimeout = () => {
                    resetUpload()
                    reject(new Error('Upload request timed out'));
                };

                xhr.send(formData);
            } catch (err) {
                reject(err)
            }
        });
    }
    
    const style: CSSProperties = {
        width: `${uploadProgress}%`
    }
    return (
        <div className="mb-2">
            <FileSelector
                enabled={!isUploading}
                onFileSelect={onFileSelect}
                onFileReset={onFileReset}
                maxSizeMB={100}
                className={file ? "mb-2" : "mb-14"} />
            {file && (
                <div className="h-12 mt-2 grid gap-2 grid-cols-[auto_1fr]">
                    <button
                        className="h-12 cursor-pointer bg-blue-400 dark:bg-blue-600 p-2 rounded-md" onClick={async () => {
                            const tokenResponse = await client.getAntiforgeryToken();
                            if (tokenResponse.ok) {
                                await uploadFile(file, tokenResponse.value!);
                            } else {
                                setError(tokenResponse.errors.join('\n'));
                            }
                        }}>Upload "{file.name}"</button>
                    
                    <div className="relative">
                        <progress
                            value={uploadProgress}
                            max="100"
                            className="absolute h-full bg-red-200 rounded-md"
                            style={style}></progress>
                    </div>
                </div>
            )}

            {error && (<p className="text-red-600 italic text-xs">{error}</p>)}
        </div>
    );
}

export default FileUploader