import * as React from 'react';
import Typography from '@material-ui/core/Typography';
import { BranchModel } from '../store';
import { createStyles, Paper } from '@material-ui/core';
import { makeStyles } from '@material-ui/styles';

const useStyles = makeStyles(() =>
  createStyles({
    root: {
      width: '100%',
      padding: '1em',
      margin: '0.5em 0'
    },
  }),
);

interface BranchProps {
  branch: BranchModel;
  showRepoName?: boolean;
}

export function Branch(props: BranchProps) {
  let { showRepoName, branch: { name, commitMessage, commitUrl, latestCommitDateTime } } = props;

  let date = new Date(latestCommitDateTime);
  let classes = useStyles();

  return (
    <Paper className={classes.root}>
      {showRepoName && <Typography variant='h5'>{props.branch.repositoryName}</Typography>}
      <Typography><b><a href={commitUrl} target='_blank' rel='noopener noreferrer'>{name}</a></b></Typography>
      <Typography variant='subtitle2'>{date.toDateString()} : {date.toTimeString()}`</Typography>
      <Typography variant='caption'>{commitMessage}</Typography>
    </Paper>
  );
}
