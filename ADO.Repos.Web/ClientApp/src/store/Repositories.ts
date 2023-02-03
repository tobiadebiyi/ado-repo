import { Action, Reducer } from 'redux';
import { AppThunkAction, Repositories, RepositoryModel, Result } from './';

// -----------------
// STATE
export interface RepositoriesState {
    isLoading: boolean;
    repositories: RepositoryModel[];
}
export interface Filters {
    HasStaleBranches: boolean;
    HasBranchesAheadOfDev: boolean;
    MainIsAheadOfDev: boolean;
}

// -----------------
// ACTIONS
interface RequestRepositoriesAction {
    type: 'REQUEST_REPOSITORIES';
}

interface RequestRepositoriesFailedAction {
    type: 'REQUEST_REPOSITORIES_FAILED';
}

interface ReceiveRepositoriesAction {
    type: 'RECEIVE_REPOSITORIES';
    repositories: RepositoryModel[];
}

type KnownAction = RequestRepositoriesAction | ReceiveRepositoriesAction | RequestRepositoriesFailedAction;

// ----------------
// ACTION CREATORS

export const actionCreators = {
    requestRepositories: (filters: Filters): AppThunkAction<KnownAction> => (dispatch) => {
        // dispatch({ type: 'RECEIVE_REPOSITORIES', repositories: Object.entries(repositories).map(r => r[1]) })

        fetch(`api/repositories?hasBranchesAheadOfDev=${filters.HasBranchesAheadOfDev}&mainIsAheadOfDev=${filters.MainIsAheadOfDev}`)
            .then(response => response.json() as Promise<Result<Repositories>>)
            .then(data => {
                data.success
                    ? dispatch({ type: 'RECEIVE_REPOSITORIES', repositories: Object.entries(data.value).map(r => r[1]) })
                    : dispatch({ type: 'REQUEST_REPOSITORIES_FAILED' });
            })
            .catch(() => dispatch({ type: 'REQUEST_REPOSITORIES_FAILED' }));

        dispatch({ type: 'REQUEST_REPOSITORIES' });
    }
};

// ----------------
// REDUCER

const unloadedState: RepositoriesState = { repositories: [], isLoading: false };

export const reducer: Reducer<RepositoriesState> = (state: RepositoriesState | undefined, incomingAction: Action): RepositoriesState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;

    switch (action.type) {
        case 'REQUEST_REPOSITORIES':
            return { ...state, isLoading: true };
        case 'RECEIVE_REPOSITORIES':
            return {
                repositories: action.repositories,
                isLoading: false
            };
        case 'REQUEST_REPOSITORIES_FAILED':
            return { ...state, isLoading: false }
        default:
            return { ...state };
    }
};
