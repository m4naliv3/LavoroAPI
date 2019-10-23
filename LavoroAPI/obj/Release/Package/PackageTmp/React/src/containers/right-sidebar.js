import React, {Component} from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import Account from '../components/account';

export default class Sidebar extends Component {
	constructor(props) {
		super(props);
		this.state = {expandSidebar: false};
	  }

	toggleSidebar() {
		this.setState({ expandSidebar: !this.state.expandSidebar })
	}

	renderIcon() {
		if (!this.state.expandSidebar) return <FontAwesomeIcon icon="bars" />
		else return <FontAwesomeIcon icon="times" />
	}

	render() {
		const sidebarClass = this.state.expandSidebar ? 'right-sidebar expanded' : 'right-sidebar';
		return (
			<div className={sidebarClass}>
				<button
					className="btn btn-primary toggle"
					onClick={() => this.toggleSidebar()}>
					{ this.renderIcon() }
				</button>
				<div className="sidebar">
					<div className="account-settings">
						<Account />
					</div>
					<div className="tools">
						<button className="btn btn-secondary btn-block">Auto-Reply</button>
						<button className="btn btn-secondary btn-block">OOO Message</button>
						<button className="btn btn-secondary btn-block">Forwarding Settings</button>
					</div>
					<div className="footer">
						<h2>What Is This?</h2>
						<p>An Enterprise-focused communication app that allows you to separate work from personal messaging on your mobile device. Lavoro Chat provides you with a localized mobile number that you can use as your "work" number and have on your business cards, email signatures, etc. SMS messages go into a chat app with productivity and organization features, and voice calls go right to your actual device.</p>
						<ul className="social">
							<li>
								<a href="https://twitter.com/lavorochat" target="_blank" rel="noopener noreferrer"><FontAwesomeIcon icon={['fab', 'twitter']} /></a>
							</li>
							<li>
								<a href="https://www.instagram.com/lavorochat/" target="_blank" rel="noopener noreferrer"><FontAwesomeIcon icon={['fab', 'instagram']} /></a>
							</li>
						</ul>
					</div>
				</div>
			</div>
		)
	}
}