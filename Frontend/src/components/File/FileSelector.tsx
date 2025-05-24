import React, {ChangeEvent, useRef, useState, FC} from "react";

type FileSelectorProps = {
    onFileSelect: (file: File) => void;
    onFileReset: () => void;
    enabled?: boolean;
    accept?: string;
    maxSizeMB?: number;
    className?: string;
};

const FileSelector: FC<FileSelectorProps> = ({ onFileSelect, onFileReset, enabled = true, accept = '*/*', maxSizeMB = 10, className = '' }) => {
    const [isDragging, setIsDragging] = useState<boolean>(false);
    const [fileName, setFileName] = useState<string>('');
    const [error, setError] = useState<string | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const maxSizeBytes = maxSizeMB * 1024 * 1024;

    const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (!files || files.length === 0 || !enabled) return;
        validateAndProcessFile(files[0]);
    };

    const validateAndProcessFile = (file: File) => {
        setError(null);

        if (file.size > maxSizeBytes) {
            setError(`File size exceeds ${maxSizeMB}MB limit`);
            return;
        }

        setFileName(file.name);
        onFileSelect(file);
    };
    
    const resetFile = () => {
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
        setError(null);
        setFileName('');
        onFileReset();
    }

    const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(true);
    };

    const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(false);
    };

    const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        setIsDragging(false);

        const files = e.dataTransfer.files;
        if (files.length === 0) return;

        validateAndProcessFile(files[0]);
    };

    const handleButtonClick = () => {
        fileInputRef.current?.click();
    };

    return (
        <div className={className}>
            <div
                onDragOver={handleDragOver}
                onDragLeave={handleDragLeave}
                onDrop={handleDrop}
                onClick={handleButtonClick}
                className={`w-full py-6 sm:rounded-lg cursor-pointer flex flex-col items-center justify-center transition-colors
          ${isDragging
                    ? 'dark:bg-blue-900 bg-blue-300'
                    : 'dark:bg-neutral-800 dark:hover:bg-neutral-600 bg-neutral-200 hover:bg-neutral-400'
                }
        `}
            >
                <input
                    disabled={!enabled}
                    type="file"
                    ref={fileInputRef}
                    onChange={handleFileChange}
                    accept={accept}
                    className="hidden"
                    data-testid="file-input"/>

                <svg
                    className="w-12 h-12 mb-2"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                    xmlns="http://www.w3.org/2000/svg">
                    <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"/>
                </svg>
                
                {fileName.length > 0 ? (
                    <div className="grid grid-rows-1 grid-cols-[1fr_1rem]">
                        <p className="text-xs">{fileName}</p>
                        <div className="relative">
                            <button
                                className="absolute left-2 -top-0.5 text-xs w-6 aspect-square cursor-pointer text-white rounded-sm bg-red-500 dark:bg-red-700 hover:bg-red-500"
                                onClick={e => {
                                    e.stopPropagation();
                                    resetFile();
                                }}>X</button>
                        </div>
                    </div>
                ) : (
                    <p className="text-xs">Select file</p>
                )}
                
                {accept !== '*/*' && (
                    <p className="text-xs mt-1">
                        Accepted formats: {accept.replace(/\./g, '').replace(/,/g, ', ')}
                    </p>
                )}
                {error && (
                    <p className="text-xs text-red-500 mt-2">{error}</p>
                )}
            </div>
        </div>
);
};

export default FileSelector;