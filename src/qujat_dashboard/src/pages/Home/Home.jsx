import React, { useState, useEffect } from "react";
import { styled, Button } from "@mui/material";
import instance from "../../axios/axios";
import { useSelector, useDispatch } from "react-redux";

import DragNDrop from "../../components/DragNDrop/DragNDrop";
import TableCategory from "../../components/Table/Table";
import AddCateoryForm from "../../components/Modals/AddCategoryForm/AddCateoryForm";
import Update from "../../components/Modals/UpdateCategory/Update";
import Delete from "../../components/Modals/Delete/Delete";

import { fetchCategory } from "../../redux/slices/categorySlice";

import "./Home.css";

const AddButton = styled(Button)({
  backgroundColor: "#16aaff",
  color: "#fff",
  padding: 11,
  "&:hover": {
    backgroundColor: "#0486d1",
  },
  "@media (max-width: 475px)": {
    width: "100%",
    marginTop: 8,
  },
});

const Home = () => {
  const dispatch = useDispatch();
  const [icons, setIcons] = useState(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [updateModalOpen, setUpdateModalOpen] = useState(false);
  const [deleteItem, setDeleteItem] = useState(false);
  const [selectedRow, setSelectedRow] = useState(null);
  const [id, setId] = useState(0);

  const { categories, isLoading } = useSelector((state) => state.categories);
  const isLoadingCategories = isLoading === true;

  const handleOpenModal = () => {
    setModalOpen(true);
  };
  const handleCloseModal = () => {
    setModalOpen(false);
  };



  return (
    <div className="home_container">
      <h1>Тақырыптық санат</h1>
      <div className="table_container">
        {/* <TableCategory /> */}
        <DragNDrop
          categories={categories}
          isOpen={modalOpen}
          handleClose={handleCloseModal}
          handleOpenModal={handleOpenModal}
        />
      </div>
      <AddCateoryForm
        icons={icons}
        isOpen={modalOpen}
        handleClose={handleCloseModal}
      />
    </div>
  );
};

export default Home;
