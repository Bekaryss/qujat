import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link, NavLink } from "react-router-dom";
import { Box } from "@mui/material";

import logo from "../../assets/logo.png";
import close from "../../assets/images/close.svg";

import { logOut } from "../../redux/slices/tokenSlice";

import { VscSignOut } from "react-icons/vsc";
import { BiCategory } from "react-icons/bi";
import { VscNotebookTemplate } from "react-icons/vsc";
import { IoStatsChartSharp } from "react-icons/io5";
import { GiScales } from "react-icons/gi";
import { IoMdAddCircle } from "react-icons/io";

import "./SideBar.css";

const SideBar = ({ isOpen, toogle }) => {
  const dispatch = useDispatch();
  const { pageIndex } = useSelector((state) => state.acts);
  const { adminInfo } = useSelector((state) => state.adminInfo);

  const handleResize = () => {
    if (window.innerWidth <= 1024) {
      toogle(!isOpen);
    }
  };

  const onClickLogOut = () => {
    if (window.confirm("Вы уверены")) {
      dispatch(logOut());
      window.localStorage.removeItem("token");
      return <Navigate to="/sign-in" />;
    }
  };

  useEffect(() => {
    handleResize();

    window.addEventListener("resize", handleResize);
    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  return (
    <aside className={`nav-container ${isOpen ? "open" : "nav_close"}`}>
      <div className="close_icon_box">
        <img
          src={close}
          className="close_icon"
          onClick={() => toogle(!isOpen)}
        />
      </div>
      <Link
        style={{ display: isOpen ? "flex" : "none" }}
        to="/"
        className="nav-logo-section"
      >
        <img src={logo} alt="logo" className="logo" />
      </Link>
      <div
        className={
          isOpen ? "sidebar_menu_container" : "sidebar_menu_hide_container"
        }
      >
        <span
          className="side_menu_head_title"
          style={{ display: isOpen ? "block" : "none" }}
        >
        </span>
        {adminInfo.userType === "RootAdmin" && (
          <NavLink
            to="/add-admin"
            className={isOpen ? "sidebar_menu" : "sidebar_menu_hide"}
          >
            <IoMdAddCircle className="sidebar_menu_icon" />
            <span
              style={{ display: isOpen ? "block" : "none" }}
              className="sidebar_menu_text"
            >
            Қолданушы
            </span>
          </NavLink>
        )}
        {menuData.map((menu) => (
          <NavLink
            key={menu.id}
            to={menu.path}
            className={isOpen ? "sidebar_menu" : "sidebar_menu_hide"}
          >
            <span className="sidebar_menu_icon">{menu.icon}</span>
            <span
              style={{ display: isOpen ? "block" : "none" }}
              className="sidebar_menu_text"
            >
              {menu.name}
            </span>
          </NavLink>
        ))}
      </div>
      <div className="sign_out_block">
        <Box
          sx={{ display: "flex", alignItems: "center" }}
          onClick={onClickLogOut}
        >
          <VscSignOut
            className="sign-out-icon"
            style={{ marginRight: "8px" }}
          />
          <span style={{ display: isOpen ? "block" : "none" }}>Шығу</span>
        </Box>
      </div>
    </aside>
  );
};

export default SideBar;

const menuData = [
  {
    id: 1,
    name: "Тақырыптық санат",
    path: "/",
    icon: <BiCategory />,
  },
  // {
  //   id: 2,
  //   name: "Тақырыптық ішкі категориялары",
  //   path: "/sub-category",
  //   icon: <MdOutlineCategory />,
  // },
  {
    id: 3,
    name: "Нормативтiк құқықтық актiлер",
    path: `/acts`,
    icon: <GiScales />,
  },
  {
    id: 4,
    name: "Құжат үлгілері",
    path: "/pattern",
    icon: <VscNotebookTemplate />,
  },
  {
    id: 5,
    name: "Статистика",
    path: "/statistics",
    icon: <IoStatsChartSharp />,
  },
];
