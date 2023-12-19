import React, { useEffect, useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import {
  Box,
  TextField,
  FormControl,
  MenuItem,
  InputLabel,
  Select,
  Modal,
  Button,
} from "@mui/material";

import { useParams } from "react-router-dom";

import close from "../../../assets/images/close.svg";
import instance from "../../../axios/axios";

const AddSubCategoryForm = ({ isOpenModal, handleClose, setDataLength }) => {
  const dispatch = useDispatch();
  const { categoryId } = useParams();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({
    defaultValues: {
      nameKz: "",
    },
    mode: "onChange",
  });

  const changeSubCategory = async (data) => {
    try {
      const response = await instance.post(
        `/api/1/categories/${categoryId}/subcategories`,
        data
      );

      if (response.status === 200) {
        closeModal();
        setDataLength((prev) => prev + 1);
      }
    } catch (error) {
      console.log(error);
    }
  };

  const onSubmit = (values) => {
    const { categoryName, ...data } = values;
    // console.log(data);
    changeSubCategory(data);
    reset();
  };

  const closeModal = () => {
    handleClose();
    reset();
  };

  useEffect(() => {}, []);

  return (
    <Modal
      open={isOpenModal}
      onClose={handleClose}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
      className="modal_container"
    >
      <div className="modal_form_wrapper update_sub_category">
        <Box
          sx={{
            width: "100%",
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            cursor: "pointer",
          }}
        >
          <h3>Жаңа ішкі тақырыптық санат қосу</h3>
          <img onClick={handleClose} src={close} className="close_icon" />
        </Box>
        <form className="add_category_form" onSubmit={handleSubmit(onSubmit)}>
          <Box sx={{ display: "flex", width: "100%", flexDirection: "column" }}>
            <Box sx={{ width: "100%", marginTop: 3 }}>
              <span className="main_text">
              Ішкі тақырыптық санат атауы
              </span>
              <TextField
                sx={{ marginTop: 1 }}
                fullWidth
                label="Ішкі тақырыптық санат атауын енгізіңіз"
                error={Boolean(errors.nameKz?.message)}
                {...register("nameKz", {
                  required: "Ішкі тақырыптық санат атауын енгізіңіз",
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
              ҚАБЫЛДАУ
            </Button>
          </Box>
        </form>
      </div>
    </Modal>
  );
};

export default AddSubCategoryForm;
