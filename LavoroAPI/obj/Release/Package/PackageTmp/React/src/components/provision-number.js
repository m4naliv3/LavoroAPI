import React, {Component} from 'react';
import { commsClient } from '../comms_client';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

class ProvisionNumber extends Component {
  render(){
    // Login Widget >> no active account >> get user info >> come here to buy a number    
    return (
      <div className="PurchaseNumber">
          <input type='text' placeholder='Area Code' />
          <button className="GetNumbers"
            onClick={_ => { 
            commsClient('PhoneResource', 'POST', {value: 661} )
                .then(phones => {
                    console.log(phones); 
                    // put this into the state and render as a list
                    // list items need to also set in state the selected number 
                });
            }} 
        />
      </div>
    );
  }
}

function mapStateToProps (state){ 
  return { 
      // if possible use local constructor in this component to assign and retrieve the area code and selected number
  } 
}

function mapDispatchToProps(dispatch) {
  return bindActionCreators({
      // if needed get the selected number to hit the purchaser EP
  }, dispatch);
}

export default connect (mapStateToProps, mapDispatchToProps)(ProvisionNumber);