import React, { Component } from 'react';
import CreateAccount from './create-account';
import LoginForm from './login-form';

export default class DeleteMe extends Component {
    constructor(props){
      super(props)
      this.state = { show: '' };
      this.checkButton = this.checkButton.bind(this);
    }

    checkButton(target) { this.setState({show: target}) }

    render() {
        if(this.state.show === 'login'){
            return (
                <div>
                    <LoginForm />
                </div>
            );    
        }
        else if(this.state.show === 'signup'){
            return (
                <div>
                    <CreateAccount />
                </div>
            );
        }
        return (
            <div>
                <input type="button" onClick={_=> this.checkButton('login')} value="Render Login Component" />
                <br />
                <input type="button" onClick={_=> this.checkButton('signup')} value="Render Signup Component" />
            </div>
        );
    }
};