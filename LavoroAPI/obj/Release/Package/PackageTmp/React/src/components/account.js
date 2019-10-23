import React, {Component} from 'react';
import {connect} from 'react-redux';

class Account extends Component {
  render(){
    if(!this.props.Account){
      return null;
    }
    return (
      <div className="user-account">
          <h2>{this.props.Account.UserName}</h2>
          <img src={this.props.Account.Avatar} alt="agent info" />
      </div>
    );
  }
}

function mapStateToProps (state){ 
  return { 
      Account: state.Account 
  } 
}

export default connect (mapStateToProps)(Account);