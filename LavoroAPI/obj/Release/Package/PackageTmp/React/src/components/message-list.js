import React, {Component} from 'react';
import { commsClient } from '../comms_client';
import {connect} from 'react-redux';
import MessageItem from './message';
import { bindActionCreators } from 'redux';
import { SetOutboundMessage } from '../actions';

class MessageList extends Component {
    renderList(items){ return items.map(m => { console.log(m);return( <MessageItem key={m.ID} Message={m} /> ) }) }

    sendMessage(){
      commsClient('OutgoingMessaging','POST', {
          MessageText: this.props.OutboundMessage, 
          Author: this.props.AccountPhone, 
          ConversationID: this.props.ConversationID
        },
        true
      ).then(this.props.SetOutboundMessage(null));
    }

    render(){
      if(!this.props.Messages) return null;
      return (
          <div>
              {this.renderList(this.props.Messages)}
              <br />
              <br />
              <input 
                type="text" 
                className="MessagingInput" 
                onInput={e => {
                  if (e.keyCode === 13) {
                    this.sendMessage()
                  }
                  this.props.SetOutboundMessage(e.target.value)}
                }
                value={this.props.OutboundMessage}
              />
              <input 
                type="button" 
                className="Send Button" 
                value="Send" 
                onClick={_ => this.sendMessage()}/>
          </div>
      )
    }
}

function mapStateToProps (state){ 
  return { 
    Messages: state.Messages,
    AccountPhone: state.AccountPhone,
    ConversationID: state.ConversationID,
    OutboundMessage: state.OutboundMessage
  } 
}
function mapDispatchToProps(dispatch) {
  return bindActionCreators({ 
    SetOutboundMessage: SetOutboundMessage
  }, dispatch);
}

export default connect (mapStateToProps, mapDispatchToProps)(MessageList);