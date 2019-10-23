import React, {Component} from 'react';
import {connect} from 'react-redux';
import { bindActionCreators } from 'redux';
import { OpenContactModal } from '../actions';
import { commsClient } from '../comms_client';

class ContactModal extends Component {
    constructor(props){
        super(props);
        this.Name = null;
        this.Title = null;
        this.Phone = null;
        this.Email = null;
        this.Avatar = null;
        this.Company = null;
        this.Favorite = false;
    }
  render(){
    if(this.props.ContactModal !== true) return null;
    return (
        <div className="ContactModal">
          <input placeholder='Contact Name' type='text' onInput={e => this.Name = e.target.value} />
          <input placeholder='Title' type='text' onInput={e => this.Title = e.target.value} />
          <input placeholder='Phone' type='text' onInput={e => this.Phone = e.target.value} />
          <input placeholder='Email' type='text' onInput={e => this.Email = e.target.value} />
          <input placeholder='Avatar' type='text' onInput={e => this.Avatar = e.target.value} />
          <input placeholder='Company' type='text' onInput={e => this.Company = e.target.value} />
          <label>Favorite</label><input placeholder='Favorite' type='checkbox' onInput={e => this.Favorite = e.target.value} />
          <input 
            type='button' 
            value='Save' 
            onClick={_ => {
                this.props.OpenContactModal(false);
                commsClient(
                    'Contacts',
                    'POST',
                    {
                        ContactName: this.Name,
                        Title: this.Title, 
                        Phone: this.Phone,
                        Email: this.Email, 
                        Avatar: this.Avatar, 
                        Company: this.Company, 
                        Favorite: this.Favorite, 
                        AccountID: 1, 
                        ProviderID: 1
                    }, 
                    true
                )
            }} 
            />
        </div>
    );
  }
}

function mapStateToProps (state){ 
  return { 
    ContactModal: state.ContactModal,
    Account: state.Account
  } 
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators({ 
      OpenContactModal: OpenContactModal
    }, dispatch);
  }
  
  export default connect (mapStateToProps, mapDispatchToProps)(ContactModal);