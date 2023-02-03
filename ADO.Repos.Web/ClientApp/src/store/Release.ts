import { Action, Reducer } from 'redux';
import { AppThunkAction, BranchModel, RepositoryModel, Result } from '.';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface ReleaseState {
    isLoading: boolean;
    release?: Release;
}

export interface Release {
    inScope: BranchModel[];
    outOfScope: RepositoryModel[];
}

// -----------------
// ACTIONS
export interface GetReleaseBranchesAction { type: 'GET_RELEASE' }
export interface ReceiveReleaseBranchesAction { type: 'RECEIVE_RELEASE', release: Release }
export interface GetReleaseBranchesFailedAction { type: 'GET_RELEASE_RAILED' }

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
export type KnownAction = GetReleaseBranchesAction | ReceiveReleaseBranchesAction | GetReleaseBranchesFailedAction;

// ----------------
// ACTION CREATORS

export const actionCreators = {
    getReleaseBranches: (releaseName: string): AppThunkAction<KnownAction> => (dispatch) => {
        // dispatch({ type: 'RECEIVE_REPOSITORIES', repositories: Object.entries(repositories).map(r => r[1]) })

        fetch(`api/release/${releaseName}`)
            .then(response => response.json() as Promise<Result<Release>>)
            .then(data => {
                data.success
                    ? dispatch({
                        type: 'RECEIVE_RELEASE',
                        release: data.value,
                    })
                    : dispatch({ type: 'GET_RELEASE_RAILED' });
            })
            .catch(() => dispatch({ type: 'GET_RELEASE_RAILED' }));

        dispatch({ type: 'GET_RELEASE' });
    }
};

// ----------------
// REDUCER

export const reducer: Reducer<ReleaseState> = (state: ReleaseState | undefined, incomingAction: Action): ReleaseState => {
    if (state === undefined) {
        return { isLoading: false, release: undefined };
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'GET_RELEASE':
            return { ...state, isLoading: true };
        case 'RECEIVE_RELEASE':
            return {
                ...state,
                release: action.release,
                isLoading: false
            };
        case 'GET_RELEASE_RAILED':
            return { ...state, isLoading: false }
        default:
            return state;
    }
};
