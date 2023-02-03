import { Badge, Checkbox, FormControlLabel, Grid, LinearProgress, TextField, Typography } from '@material-ui/core';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import * as React from 'react';
import { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Button, FormGroup } from 'reactstrap';
import { ApplicationState, BranchModel } from '../store';
import * as ReleaseStore from '../store/Release';
import { Branch } from './Branch';
import Accordion, { AccordionSection } from './Accordion';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        root: {
            width: '100%',
        },
        container: {
            flexDirection: "column"
        },
        form: { display: 'flex', marginBottom: '2em' },
        releaseNameTextField: { marginRight: '1em' },
        badge: {}
    }),
);

type ReleaseProps =
    ReleaseStore.ReleaseState
    & typeof ReleaseStore.actionCreators
    & RouteComponentProps<{}>;

function Release(props: ReleaseProps) {
    const classes = useStyles();
    const [releaseName, setReleaseName] = useState("");
    const [aheadOfDev, setAheadOfDev] = useState(false);
    const [isLocked, setIsLocked] = useState(false);
    const [inScope, setInScope] = useState<BranchModel[]>([]);
    let { isLoading, release } = props;

    useEffect(filter, []);

    useEffect(() => {
        filter();
    }, [aheadOfDev, isLocked]);

    function filter() {
        if (release != undefined) {
            var branches = release.inScope;
            if (aheadOfDev)
                branches = branches.filter(b => b.isAheadOfDev);

            if (isLocked)
                branches = branches.filter(b => b.isLocked);

            setInScope(branches);
        }
    }

    if (isLoading)
        return <LinearProgress variant='indeterminate' />;

    function getAccordionEntries(releaseModel: ReleaseStore.Release): AccordionSection[] {
        return ([
            {
                title: 'In Scope',
                details: (
                    <React.Fragment>
                        {inScope.map((b, k) => (<Branch key={k} branch={b} showRepoName />))}
                    </React.Fragment>
                ),
                summary: (
                    <Badge color="secondary" badgeContent={inScope.length}>
                        <Typography variant='h5'>In Scope</Typography>
                    </Badge>
                )
            },
            {
                title: 'Out Of Scope',
                details: (
                    <React.Fragment>
                        {releaseModel.outOfScope.map((repo, k) => (<Typography key={k} variant='subtitle1'><a href={repo.url}>{repo.name}</a></Typography>))}
                    </React.Fragment>
                ),
                summary: (
                    <React.Fragment>
                        <Badge color="secondary" badgeContent={releaseModel.outOfScope.length}>
                            <Typography variant='h5'>Out Of Scope</Typography>
                        </Badge>
                    </React.Fragment>
                ),
            }
        ]);
    }

    let handleReturn = (e: React.KeyboardEvent<HTMLDivElement>) => {
        if (e.keyCode === 13) {
            handleFindReleaseClicked();
        }
    }

    let handleFindReleaseClicked = () => props.getReleaseBranches(releaseName);

    return (
        <div className={classes.root}>
            <h1>Release</h1>
            <Grid container className={classes.container}>
                <Grid item className={classes.form}>
                    <FormGroup>
                        <TextField value={releaseName} onChange={e => setReleaseName(e.target.value)} className={classes.releaseNameTextField} onKeyUp={handleReturn} />
                    </FormGroup>
                    <Button onClick={handleFindReleaseClicked}>Find Release</Button>
                </Grid>
                <Grid item>
                    {release && (
                        <div>
                            <FormGroup>
                                <FormControlLabel control={<Checkbox checked={aheadOfDev} onChange={e => setAheadOfDev(e.target.checked)} />} label="Ahead of dev" />
                                <FormControlLabel control={<Checkbox checked={isLocked} onChange={e => setIsLocked(e.target.checked)} />} label="Locked" />

                                <Accordion entries={getAccordionEntries(release)} />
                            </FormGroup>
                        </div>
                    )}
                </Grid>
            </Grid>
        </div>
    );
}

export default connect(
    (state: ApplicationState) => state.release,
    ReleaseStore.actionCreators
)(Release as any);
