import React from 'react';
import './Footer.scss';
import { NavLink } from 'react-router-dom';

function Footer() {
  return (
    <footer className="footer">
      <div className="footer-wrapper">
        <div className={'footer-content'}>
          <div className="main-footer-container">
            <h4 className='main-footer-text'>Qujat</h4>
            <p>Құжат айналымының автоматтардырылған электронды базасы</p>
          </div>
          <div className="secondary-container">
            <h4 className='secondary-text'>Бөлімдер</h4>
            <div className='page-link-container'>
              <NavLink to='/' className={'custom-link'}>Құжаттар топтамасы</NavLink>
              <NavLink to='/legal-grounds' className={'custom-link'}>Құқықтық негіздер</NavLink>
              <NavLink to='/open-api' className={'custom-link'}>Ашық API</NavLink>
            </div>
          </div>
          <div className="links-container">
            <h4 className='links-text'>Басқа жобаларымыз</h4>
            <div className='links'>
              <a href="https://abai.institute/">abai.institute</a>
              <a href="http://balatili.kz/">balatili.kz</a>
              <a href="https://tilalemi.kz/">tilemi.kz</a>
              <a href="https://www.qazlatyn.kz/">qazlatyn</a>
              <a href="https://emle.kz/">emle.kz</a>
              <a href="https://tilqural.kz/">tilqural.kz</a>
              <a href="https://sozdikqor.kz/">sozdikqor.kz</a>
              <a href="https://termincom.kz/">terminkom.kz</a>
            </div>
          </div>
        </div>
        <div className="footer-bottom">
          © 2023 “Qujat.kz” Барлық құқықтар сақталған.
        </div>
      </div>
    </footer>
  );
}

export default Footer;
