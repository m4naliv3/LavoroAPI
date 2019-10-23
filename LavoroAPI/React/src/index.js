import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faMobileAlt, faBars, faTimes, faPlus, faMinus } from '@fortawesome/free-solid-svg-icons';
import { faInstagram, faTwitter } from '@fortawesome/free-brands-svg-icons';
import './App.scss';
import {store} from './store'
import Dashboard from './dashboard';

// Builds FontAwesome Library to use font icons throughout
library.add(faMobileAlt, faBars, faTimes, faPlus, faMinus, faInstagram, faTwitter);

ReactDOM.render(
  <Provider store={store}>
      <Dashboard />
  </Provider>,
  document.getElementById('root')
);