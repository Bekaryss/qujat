import React from "react";
import { useDispatch } from "react-redux";
import { Navigate } from "react-router-dom";
import { Menu, MenuItem } from "@mui/material";
import { VscSignOut } from "react-icons/vsc";

import { logOut } from "../../redux/slices/tokenSlice";

import "./ProfileInfo.css";

const ProfileInfo = ({ anchorEl, open, handleClose, handleClick }) => {
  const dispatch = useDispatch();
  const onClickLogOut = () => {
    if (window.confirm("Вы уверены")) {
      dispatch(logOut());
      window.localStorage.removeItem("token");
      return <Navigate to="/sign-in" />;
    }
  };

  return (
    <Menu
      anchorEl={anchorEl}
      id="account-menu"
      open={open}
      onClose={handleClose}
      onClick={handleClose}
      sx={{ marginTop: 1 }}
      MenuListProps={{
        "aria-labelledby": "basic-button",
      }}
    >
      <MenuItem onClick={onClickLogOut} className="user_menu_item">
        <VscSignOut className="icon_menu" />
        Шығу
      </MenuItem>
    </Menu>
  );
};

export default ProfileInfo;
