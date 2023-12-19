import React, { useState } from "react";
import { useForm } from "react-hook-form";

import { Box, TextField, Modal, Button, FormControl } from "@mui/material";

import close from "../../../assets/images/close.svg";
import instance from "../../../axios/axios";
const AddActs = ({ modalOpenAct, handleCloseModal, UpdateData }) => {
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isValid },
    reset,
  } = useForm({
    defaultValues: {
      nameKz: "",
      uri: "",
    },
    mode: "onChange",
  });

  const AddActs = async (data) => {
    try {
      const response = await instance.post("/api/1/links", data);

      if (response.status === 200) {
        UpdateData();
        handleCloseModal();
        reset();
        return response;
      }
    } catch (error) {
      console.log(error);
    }
  };

  const onSubmit = (values) => {
    AddActs(values);
  };
  return (
    <Modal
      open={modalOpenAct}
      onClose={handleCloseModal}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
      className="modal_container"
    >
      <div className="modal_form_wrapper">
        <Box
          sx={{
            width: "100%",
            display: "flex",
            align: "center",
            justifyContent: "space-between",
          }}
        >
          <h3>Жаңа нормативтiк құқықтық актiні қосу</h3>
          <img onClick={handleCloseModal} src={close} className="close_icon" />
        </Box>
        <form className="add_category_form" onSubmit={handleSubmit(onSubmit)}>
          <Box sx={{ display: "flex", width: "100%", flexDirection: "column" }}>
            <Box sx={{ width: "100%", marginTop: 2 }}>
              <span className="main_text">
                Нормативтiк құқықтық актiнің қазакша атын енгізіңіз
              </span>
              <TextField
                sx={{ marginTop: 1 }}
                fullWidth
                label="Нормативтiк құқықтық актiнің қазакша атын енгізіңіз"
                error={Boolean(errors.nameKz?.message)}
                {...register("nameKz", {
                  required:
                    "Нормативтiк құқықтық актiнің қазакша атын енгізіңіз",
                })}
              />
            </Box>
            <Box sx={{ width: "100%", marginTop: 2 }}>
              <span className="main_text">
                Нормативтік құжаттама веб-сайтына сілтеме
              </span>

              <FormControl fullWidth>
                <TextField
                  sx={{ marginTop: 1 }}
                  type="url"
                  label="Нормативтік құжаттама веб-сайтына сілтеме"
                  error={Boolean(errors.categoryName?.message)}
                  {...register("uri", {
                    required: "Нормативтік құжаттама веб-сайтына сілтеме",
                  })}
                />
              </FormControl>
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

export default AddActs;
