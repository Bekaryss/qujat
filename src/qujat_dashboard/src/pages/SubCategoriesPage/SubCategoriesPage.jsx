import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";

import { Box } from "@mui/material";

import DragNDropSubCategories from "./DragNDropSubCategories/DragNDropSubCategories";
import { getCurrentCategory } from "../../redux/slices/currentCategory";
import { MdOutlineArrowBackIosNew } from "react-icons/md";

const SubcategoriesPage = () => {
  const dispatch = useDispatch();
  const { categoryId } = useParams();
  const { category, isLoading } = useSelector((state) => state.category);

  const navigation = useNavigate();

  const getBack = () => {
    navigation("/");
  };

  useEffect(() => {
    dispatch(getCurrentCategory(categoryId));
  }, [dispatch, categoryId]);

  return (
    <>
      {!isLoading && (
        <div className="home_container">
          <div>
            <h1>{category?.nameKz}</h1>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                transition: "var(--tran-03)",
                "&:hover": {
                  color: "var(--main-color-btn)", // Fixing the typo here
                },
                cursor: "pointer",
              }}
              onClick={() => getBack()}
            >
              <MdOutlineArrowBackIosNew />
              <span className="main_text" style={{ paddingLeft: "12px" }}>
                Артқа
              </span>
            </Box>
          </div>
          <DragNDropSubCategories />
        </div>
      )}
    </>
  );
};

export default SubcategoriesPage;
