import * as React from 'react';
import { default as MuiAccordion } from '@material-ui/core/Accordion';
import AccordionDetails from '@material-ui/core/AccordionDetails';
import AccordionSummary from '@material-ui/core/AccordionSummary';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        root: {
            width: '100%',
        },
        details: {
            flexWrap: 'wrap',
            justifyContent: 'space-around',
        }
    }),
);

export interface AccordionSection {
    title: string;
    summary: JSX.Element;
    details: JSX.Element;
}

interface AccordionProps {
    entries: AccordionSection[]
}

function Accordion(props: AccordionProps) {
    let { entries } = props;

    let classes = useStyles();

    const [expanded, setExpanded] = React.useState<string | false>(false);

    const handleChange = (panel: string) => (event: React.ChangeEvent<{}>, isExpanded: boolean) => {
        setExpanded(isExpanded ? panel : false);
    };

    return (
        <React.Fragment>
            {entries.map((e, k) => (
                <MuiAccordion key={k} expanded={expanded === `accord-${e.title}`} onChange={handleChange(`accord-${e.title}`)}>
                    <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel1bh-content"
                        id="panel1bh-header"
                    >
                        {e.summary}
                    </AccordionSummary>
                    <AccordionDetails className={classes.details}>
                        {e.details}
                    </AccordionDetails>
                </MuiAccordion>
            ))}
        </React.Fragment>
    );
}

export default Accordion;