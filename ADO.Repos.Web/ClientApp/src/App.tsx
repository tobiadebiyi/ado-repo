import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Repositories from './components/Repositories';
import Release from './components/Release';

import './custom.css'

export default () => (
    <Layout>
        <Route exact path='/' component={Repositories} />
        <Route path='/release/:releaseName?' component={Release} />
    </Layout>
);
