import React from 'react';
import './Navbar.scss';
import { NavLink, useNavigate } from 'react-router-dom';

const Navbar = () => {
    const navigate = useNavigate()
    
    return (
        <nav className={'navbar'}>
            <div className={'header-wrapper'}>
                <span className={'header'} onClick={() => navigate('/')}>Qujat</span>
                <div></div>
            </div>
            <div className={'links-wrapper'}>
                <NavLink to='/' className={'custom-link'}>Басты бет</NavLink>
                <NavLink to='/legal-grounds' className={'custom-link'}>Құқықтық негіздер</NavLink>
                <NavLink to='/open-api' className={'custom-link'}>Ашық API</NavLink>
            </div>
        </nav>
    );
};

export default Navbar;