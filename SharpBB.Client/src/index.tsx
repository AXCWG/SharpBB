/* @refresh reload */
import './index.css';
import { render } from 'solid-js/web';
import 'solid-devtools';

import Index from './App';
import { Route, Router } from '@solidjs/router';
import { lazy } from 'solid-js';

const root = document.getElementById('root');
const ForumIndex = lazy(() => import("./App"))
if (import.meta.env.DEV && !(root instanceof HTMLElement)) {
  throw new Error(
    'Root element not found. Did you forget to add it to your index.html? Or maybe the id attribute got misspelled?',
  );
}

render(() => <Router>
  <Route path="/" component={ForumIndex} />
</Router>, root!);
