import React, { Component, PropTypes } from 'react';
import classnames from 'classnames';

import './FriendListItem.scss';

export default class FriendListItem extends Component {
  static propTypes = {
    deleteFriend: PropTypes.func.isRequired,
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    starFriend: PropTypes.func.isRequired,
    starred: PropTypes.bool
  };

  render() {
    return (
      <li className="friendListItem">
        <div className="friendInfos">
          <div className="btn btn-default btnAction" onClick={() => this.props.starFriend(this.props.id)}>
            <h1><span className="fa fa-user"></span></h1>
            <div><span>{this.props.name}</span></div>
          </div>
        </div>
      </li>
    );
  }
}
