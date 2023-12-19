import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import instance from "../../../axios/axios";
import { Modal, Box, TextField, Button } from "@mui/material";

import { fetchSubCategories } from "../../../redux/slices/subCategoriesSlice";

import close from "../../../assets/images/close.svg";

const UpdateSubCategory = ({
  isOpenUpdate,
  handleCloseUpdate,
  subCategoryId,
}) => {
  const dispatch = useDispatch();
  const { categoryId } = useParams();

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
    reset,
  } = useForm({
    defaultValues: {
      nameKz: "",
    },
    mode: "onBlur",
  });

  const updateSubCategory = async (data) => {
    try {
      const response = await instance.patch(
        `/api/1/categories/${categoryId}/subcategories/${subCategoryId}`,
        data
      );

      if (response.status === 200) {
        dispatch(fetchSubCategories(categoryId));
        handleCloseUpdate();
      }
    } catch (e) {
      console.log(e);
    }
  };

  const onSubmit = (values) => {
    console.log(values);
    updateSubCategory(values);
    reset();
  };

  useEffect(() => {
    const getSubCategory = async () => {
      try {
        const response = await instance.get(
          `/api/1/categories/${categoryId}/subcategories/${subCategoryId}`
        );

        if (response.status == 200) {
          setValue("nameKz", response.data.responseData.nameKz);
          console.log(response);
        }
      } catch (error) {
        console.log(error);
      }
    };
    if (subCategoryId !== undefined && subCategoryId !== 0) {
      getSubCategory();
    }
  }, [subCategoryId, categoryId]);

  return (
    <Modal
      open={isOpenUpdate}
      onClose={handleCloseUpdate}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
      className="modal_container"
    >
      <div className="modal_form_wrapper update_sub_category">
        <Box
          sx={{
            width: "100%",
            height: "100%",
            display: "flex",
            flexDirection: "column",
          }}
        >
          <Box
            sx={{
              width: "100%",
              display: "flex",
              align: "center",
              justifyContent: "space-between",
            }}
          >
            <h3>Өзгерту</h3>
            <img
              onClick={handleCloseUpdate}
              src={close}
              className="close_icon"
            />
          </Box>

          <form className="add_category_form" onSubmit={handleSubmit(onSubmit)}>
            <Box
              sx={{
                width: "100%",
                marginTop: 2,
                display: "flex",
                flexDirection: "column",
              }}
            >
              <Box sx={{ width: "100%", marginTop: 3 }}>
                <span className="main_text">Тақырыптын аты</span>
                <TextField
                  sx={{ marginTop: 2 }}
                  fullWidth
                  error={Boolean(errors.categoryName?.message)}
                  {...register("nameKz", {
                    required: "Тақырыптын атын енгізіңіз",
                  })}
                />
              </Box>
            </Box>
            <Box
              sx={{
                width: "100%",
                display: "flex",
                alignItems: "center",
                marginTop: 2,
              }}
            >
              <Button
                color="success"
                variant="contained"
                sx={{ width: "100%" }}
                type="submit"
              >
                Жаңарту
              </Button>
            </Box>
          </form>
        </Box>
      </div>
    </Modal>
  );
};

export default UpdateSubCategory;
