import React, { Component } from 'react';

export default class LoginForm extends Component {
  constructor(props){
    super(props)
    this.state = {username: '', password: ''};
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  handleSubmit(event) {
    event.preventDefault();

    console.log(this.state.username, ' --> ', this.state.password) 
  }

  handleChange(target, event) {
    if(target === 'username') this.setState({username: event.target.value});
    if(target === 'password') this.setState({password: event.target.value});
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
            type="password"
            onChange={(e)=>{ this.handleChange('password', e) }}
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