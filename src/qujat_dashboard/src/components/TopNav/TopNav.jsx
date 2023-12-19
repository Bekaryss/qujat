import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import { Avatar, Typography } from "@mui/material";

import burgerIcon from "../../assets/images/burger.png";

import ProfileInfo from "../ProfileInfo/ProfileInfo";
import { selectIsAuth } from "../../redux/slices/tokenSlice";

import "./TopNav.css";

const TopNav = ({ isOpen, toogle }) => {
  //open dropdown menu
  const [anchorEl, setAnchorEl] = React.useState(null);
  const open = Boolean(anchorEl);

  const { adminInfo } = useSelector((state) => state.adminInfo);

  const isAuth = useSelector(selectIsAuth);

  //handleClick open and close user menu
  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  useEffect(() => {}, [adminInfo]);

  return (
    <div className="header_main_content">
      <div className="nav_toggle">
        <img
          src={burgerIcon}
          style={{ left: isOpen ? "90%" : "30.5%" }}
          className="burger-btn"
          onClick={() => toogle()}
        />
      </div>
      <div className="navbar">
        {adminInfo && (
          <Typography variant="subtitle1" sx={{ marginRight: 1, fontWeight: "bold" }}>
            {adminInfo.email}
          </Typography>
        )}

        <div className="user_info" onClick={handleClick}>
          <Avatar sx={{ cursor: "pointer" }}></Avatar>
        </div>
        <ProfileInfo
          anchorEl={anchorEl}
          open={open}
          handleClick={handleClick}
          handleClose={handleClose}
        />
      </div>
    </div>
  );
};

export default TopNav;
