import React from "react";
import { Navigate } from "react-router-dom";

import SideBar from "../SideBar/SideBar";
import Router from "../../routes/Router";
import TopNav from "../TopNav/TopNav";

import "./Layout.css";

const Layout = () => {
  const [isOpen, setIsOpen] = React.useState(true);

  const toogle = () => {
    setIsOpen(!isOpen);
  };

  return (
    <div className="layout_container">
      <SideBar isOpen={isOpen} toogle={toogle} />
      <div
        style={{ paddingLeft: isOpen ? "250px" : "80px" }}
        className="main__layout"
      >
        <TopNav isOpen={isOpen} toogle={toogle} />
        <div className="content">
          <Router />
        </div>
      </div>
    </div>
  );
};

export default Layout;
