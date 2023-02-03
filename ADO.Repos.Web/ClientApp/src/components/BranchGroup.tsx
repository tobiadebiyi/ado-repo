import * as React from 'react';
import Typography from '@material-ui/core/Typography';
import { Grid } from '@material-ui/core';
import { Branch } from './Branch';
import { BranchModel } from '../store';

interface BranchGroupProps {
  branches: BranchModel[];
  groupName: string;
}
export function BranchGroup(props: BranchGroupProps) {
  let { groupName, branches } = props;
  return (
    <Grid item xs={12}>
      <Typography variant='h5'>{groupName}</Typography>
      <ul>
        {branches.map(b => (
          b !== null &&
          <li key={b.name}>
            <Branch branch={b} />
          </li>
        ))}
      </ul>
    </Grid>
  );
}
