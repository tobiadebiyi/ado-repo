import * as React from 'react';
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles';
import Accordion from '@material-ui/core/Accordion';
import AccordionDetails from '@material-ui/core/AccordionDetails';
import AccordionSummary from '@material-ui/core/AccordionSummary';
import Typography from '@material-ui/core/Typography';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';

import { Checkbox, FormControlLabel, FormGroup, Grid, LinearProgress } from '@material-ui/core';
import { useState } from 'react';

import * as RepositoriesStore from '../store/Repositories';
import { ApplicationState } from '../store';
import { connect } from 'react-redux';
import { BranchGroup } from './BranchGroup';
import { Button } from 'reactstrap';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      width: '100%',
    },
    heading: {
      fontSize: theme.typography.pxToRem(15),
      flexBasis: '33.33%',
      flexShrink: 0,
    },
    secondaryHeading: {
      fontSize: theme.typography.pxToRem(15),
      color: theme.palette.text.secondary,
    },
    paper: {},
    control: {},
    horizontalContainer: { flexDirection: "row" },
    filters: { flex: 1 },
    filterActions: { display: "flex", alignItems: "flex-end" },
  }),
);

type RepositoriesProps =
  RepositoriesStore.RepositoriesState
  & typeof RepositoriesStore.actionCreators;

function ControlledAccordions(props: RepositoriesProps) {
  const classes = useStyles();
  const [expanded, setExpanded] = React.useState<string | false>(false);


  const handleChange = (panel: string) => (event: React.ChangeEvent<{}>, isExpanded: boolean) => {
    setExpanded(isExpanded ? panel : false);
  };

  let ensureDataFetched = () => {
    requestRepositories({ MainIsAheadOfDev: mainIsAheadOfDev, HasBranchesAheadOfDev: hasBranchesAheadOfDev, HasStaleBranches: false });
  };

  const [hasBranchesAheadOfDev, setAheadOfDev] = useState(false)
  const [mainIsAheadOfDev, setMainAheadOfDev] = useState(false)

  let { repositories, isLoading, requestRepositories } = props;

  if (isLoading)
    return <LinearProgress variant='indeterminate' />;

  return (
    <div className={classes.root}>
      <h1>Repositories</h1>
      <Grid container spacing={2}>
        <Grid item xs={12}>
          <Grid container className={classes.horizontalContainer}>
            <Grid item className={classes.filters}>
              <FormGroup>
                <FormControlLabel control={<Checkbox checked={mainIsAheadOfDev} onChange={e => setMainAheadOfDev(e.target.checked)} />} label="Main branch is ahead of dev" />
                <FormControlLabel control={<Checkbox checked={hasBranchesAheadOfDev} onChange={e => setAheadOfDev(e.target.checked)} />} label="Has branches ahead of dev" />
              </FormGroup>
            </Grid>
            <Grid item className={classes.filterActions}>
              <Button onClick={ensureDataFetched}>Apply Filters</Button>
            </Grid>
          </Grid>
        </Grid>
        <Grid item xs={12}>
          {repositories.map(repo => {
            return (
              <Accordion key={repo.name} expanded={expanded === `accord-${repo.name}`} onChange={handleChange(`accord-${repo.name}`)}>
                <AccordionSummary
                  expandIcon={<ExpandMoreIcon />}
                  aria-controls="panel1bh-content"
                  id="panel1bh-header"
                >
                  <Typography className={classes.heading}><a href={repo.url} target='_blank' rel='noopener noreferrer'> {repo.name} </a></Typography>
                </AccordionSummary>
                <AccordionDetails>
                  <Grid container className={classes.root} spacing={2}>
                    {repo.targetBranches.length > 0 && <BranchGroup groupName='Target Branches' branches={repo.targetBranches} />}
                    {repo.branchesAheadOfDev.length > 0 && <BranchGroup groupName='Branches Ahead of Dev' branches={repo.branchesAheadOfDev} />}
                  </Grid>
                </AccordionDetails>
              </Accordion>
            );
          })}
        </Grid>
      </Grid>
    </div >
  );
}

export default connect(
  (state: ApplicationState) => state.repositories,
  RepositoriesStore.actionCreators
)(ControlledAccordions as any);