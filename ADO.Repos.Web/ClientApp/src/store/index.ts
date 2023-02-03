import * as Release from './Release';
import * as Repo from '../store/Repositories';

// The top-level state object
export interface ApplicationState {
    release: Release.ReleaseState | undefined;
    repositories: Repo.RepositoriesState | undefined;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    release: Release.reducer,
    repositories: Repo.reducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}

// Types and interfaces
export type Repositories = { [name: string]: RepositoryModel };

export interface BranchModel {
    name: string;
    latestCommitDateTime: string;
    commiterName: string;
    commitMessage: string;
    commitUrl: string;
    repositoryName: string;
    commitId: string;
    isAheadOfDev: boolean;
    isLocked: unknown;
}

export interface RepositoryModel {
    name: string;
    url: string;
    targetBranches: BranchModel[];
    staleBranches: BranchModel[] | null;
    branchesAheadOfDev: BranchModel[];
    mainIsAheadOfDev: boolean;
}
export interface Result<T> {
    success: boolean;
    value: T;
    messages: string[];
    messagesSummary: string;
}