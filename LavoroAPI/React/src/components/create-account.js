import React, { Component } from 'react';
import { commsClient } from '../comms_client';

export default class CreateAccount extends Component {
  constructor(props){
    super(props)
    this.state = {
        username: '', 
        password: '',
        businessName: '',
        email: '',
        avatar: ''
    };
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  handleSubmit(event) {
    event.preventDefault();
    commsClient('Signup','POST', { 
        Username: this.state.username, 
        Password: this.state.password,
        BusinessName: this.state.businessName,
        Email: this.state.email,
        Avatar: this.state.avatar
    }, true)
    console.log(this.state) 
  }

  handleChange(target, event) {
    if(target === 'username') this.setState({username: event.target.value});
    if(target === 'password') this.setState({password: event.target.value});
    if(target === 'businessName') this.setState({businessName: event.target.value});
    if(target === 'email') this.setState({email: event.target.value});
    if(target === 'avatar') this.setState({avatar: event.target.value});
  }

  render() {
    return (
      <form onSubmit={this.handleSubmit}>
        <label>
            Username:
            <input
            id="username" 
            type="text"
            value={this.username}
            onChange={(e)=>{ this.handleChange('username', e) }}
            />
            Password:
            <input
            id="password" 
            type="text"
            onChange={(e)=>{ this.handleChange('password', e) }}
            />
            Business Name:
            <input
            id="businessName" 
            type="text"
            onChange={(e)=>{ this.handleChange('businessName', e) }}
            />
            Email Address:
            <input
            id="email" 
            type="text"
            onChange={(e)=>{ this.handleChange('email', e) }}
            />
            Avatar Url:
            <input
            id="avatar" 
            type="text"
            onChange={(e)=>{ this.handleChange('avatar', e) }}
            />
        </label>
        <input id="submit" 
        type="submit" 
        value="Submit" 
        />
      </form>
    );
  }
};      