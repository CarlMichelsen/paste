import {FC, useState} from "react";
import BurgerMenuBlack from "../../assets/icons/burger-menu-black.svg"
import BurgerMenuWhite from "../../assets/icons/burger-menu-white.svg"
import {FileDto} from "../../model/file/fileDto.ts";
import {useDarkMode} from "../../hooks.ts";
import FileCardDetails from "./FileCardDetails.tsx";

type FileCardProps = {
    file: FileDto;
}

const FileCard: FC<FileCardProps> = ({ file }) => {
    const [open, setOpen] = useState<boolean>(false);
    const darkMode = useDarkMode();
    
    const openClasses = (isOpen: boolean) =>
        isOpen ? "grid-rows-2 grid-rows-[2.5rem_1fr] grid-cols-[1fr_2rem] h-24" : "grid-rows-1 grid-cols-[1fr_2rem] h-10";
    
    return (
        <span className={`transition-transform grid ${openClasses(open)}`}>
            <div className={`text-xl transition-colors bg-neutral-200 hover:bg-neutral-400 dark:bg-neutral-800 dark:hover:bg-neutral-600 ${open ? 'rounded-tl-sm' : 'rounded-l-sm'} px-2 pt-1`}>
                <h3>{file.name}</h3>
            </div>
            <button
                onClick={() => setOpen(!open)}
                className={`text-xl transition-colors border-r border-b border-t border-neutral-300 dark:border-neutral-700 bg-neutral-300 hover:bg-neutral-50 dark:bg-neutral-900 dark:hover:bg-neutral-700 hover:text-white cursor-pointer ${open ? 'rounded-tr-sm' : 'rounded-r-sm'}`}>
                <img src={darkMode ? BurgerMenuWhite : BurgerMenuBlack} alt="burger-menu"/>
            </button>
            {open ? (
                <div className="border-b border-l border-r rounded-b-sm col-span-2 border-neutral-300 dark:border-neutral-700">
                    <FileCardDetails file={file} />
                </div>
            ) : null}
        </span>
    )
}

export default FileCard