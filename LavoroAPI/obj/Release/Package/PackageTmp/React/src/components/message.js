import React, {Component} from 'react';

export default class MessageItem extends Component {
  render(){
    var message = this.props.Message;
    if(!message) return null;
    return (
      <div className="Message-Item">
        <span>{message.MessageText}</span>
        <br/>
        <span>{message.Author}</span>
        <span>{message.SentDate}</span>
      </div>
    );
  }
}