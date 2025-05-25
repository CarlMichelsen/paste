// src/store/useStore.ts
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'
import {AuthenticatedUser} from "../model/user.ts";

type State = {
    user: AuthenticatedUser | null;
}

type Actions = {
    setUser: (user: State['user']) => void
}

export const applicationStore = create<State & Actions>()(
    immer((set) => ({
        // Initial state
        user: null,

        // Actions
        setUser: (user) => set((state) => {
            state.user = user
        })
    }))
);