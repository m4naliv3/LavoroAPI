import React, {Component} from 'react';
import { SetMessages, SetAccountPhone, SetAccount, OpenContactModal } from './actions';
import {connect} from 'react-redux';
import { bindActionCreators } from 'redux';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import Sidebar from './containers/right-sidebar';
import ContactList from './components/contact-list';
import Header from './components/header';
import MessageList from './components/message-list';
import ContactModal from './components/contact-modal';
import DeleteMe from './components/delete-me';

class Dashboard extends Component {
  render(){
    // do something with the component did mount to preload all of this crap once we have the account id back
    // we can se the contacts, phone number, etc.
    if(!this.props.Account) {
      this.props.SetAccount(2)
      
      /*
      commsClient('accounts/1').then(r => { this.props.SetAccount(r) }); 
      commsClient('phones/1').then(r => { this.props.SetAccountPhone(r.PhoneNumber) }); 
      */
    }

    return (
      <div className="main-dashboard">
        <div className="container">
          <Header />
          <div className="content">
            <div className="contacts">
              <div className="title">
                <h2>My Contacts</h2>
              </div>
              <div className="contact-tools">
                <div onClick={_ => this.props.OpenContactModal(true)}><FontAwesomeIcon icon="plus" /></div>
                <FontAwesomeIcon icon="minus" />
              </div>
              <div className="ui-scroll">                
                <div className="contact-list">
                  <ContactModal />
                  <ContactList />
                </div>
              </div>
            </div>      
            <div className="messaging">
              <div className="messaging-tools">
                <button className="btn btn-tab btn-square">Remind Me</button>
                <button className="btn btn-tab btn-square">Move to Email</button>
                <button className="btn btn-tab btn-square">Calendar</button>
                <button className="btn btn-tab btn-square">Search</button>
                <button className="btn btn-tab btn-square">Help</button>
              </div>
              <div className="ui-scroll"> 
                <div className="message-list">
                <MessageList />
                </div>
              </div>
            </div>        
          </div>
        </div>
        <Sidebar />
      </div>
    );
  }
}

function mapStateToProps (state){ 
  return { 
      Messages: state.Messages, 
      Account: state.Account,
      AccountPhone: state.AccountPhone
  } 
}

function mapDispatchToProps(dispatch) {
  return bindActionCreators({ 
    OpenContactModal: OpenContactModal,
    SetAccount: SetAccount,
    SetMessages: SetMessages,
    SetAccountPhone: SetAccountPhone
  }, dispatch);
}

export default connect (mapStateToProps, mapDispatchToProps)(Dashboard);