import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useForm } from "react-hook-form";

import {
  Box,
  TextField,
  FormControl,
  MenuItem,
  InputLabel,
  Select,
  Modal,
  Button,
  Input,
} from "@mui/material";

import close from "../../../assets/images/close.svg";
import instance from "../../../axios/axios";

const UpdateActs = ({ modalUpdateAct, handleCloseUpdateModal, UpdateData }) => {
  const [currentAct, setCurrentAct] = useState(null);
  const { id } = useSelector((state) => state.acts);

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isValid },
    reset,
    setValue,
  } = useForm({
    defaultValues: {
      nameKz: "",
      uri: "",
    },
    mode: "onChange",
  });

  const upDateActs = async (actId, data) => {
    try {
      const response = await instance.patch(`/api/1/links/${actId}`, data);

      if (response.status === 200) {
        handleCloseUpdateModal();
        UpdateData();
      }
    } catch (e) {
      console.log(e);
    }
  };

  const onSubmit = (values) => {
    upDateActs(id, values);
  };

  useEffect(() => {
    const getActById = async (actId) => {
      console.log(actId);
      try {
        const response = await instance.get(`/api/1/links/${actId}`);
        if (response.status === 200) {
          setCurrentAct(response.data);
          setValue("nameKz", response.data.responseData.nameKz);
          setValue("uri", response.data.responseData.uri);
        }
      } catch (error) {
        console.log(error);
      }
    };

    if (id !== 0) {
      getActById(id);
    }
  }, [id]);

  useEffect(() => {}, [currentAct]);

  return (
    <Modal
      open={modalUpdateAct}
      onClose={handleCloseUpdateModal}
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
          <h3>Жаңа құжат үлгісін қосу</h3>
          <img
            onClick={handleCloseUpdateModal}
            src={close}
            className="close_icon"
          />
        </Box>
        <form className="add_category_form" onSubmit={handleSubmit(onSubmit)}>
          <Box sx={{ display: "flex", width: "100%", flexDirection: "column" }}>
            <Box sx={{ width: "100%", marginTop: 2 }}>
              <span className="main_text">
                Нормативтiк құқықтық актiнің қазакша аты
              </span>
              <TextField
                sx={{ marginTop: 1 }}
                fullWidth
                error={Boolean(errors.categoryName?.message)}
                {...register("nameKz", {
                  required: "Нормативтiк құқықтық актiнің атын енгізіңіз",
                })}
              />
            </Box>
            <Box sx={{ width: "100%", marginTop: 3 }}>
              <span className="main_text">
                Нормативтік құжаттама веб-сайтына сілтеме
              </span>

              <FormControl fullWidth sx={{ marginTop: 2 }}>
                <TextField
                  id="uri"
                  type="url"
                  error={Boolean(errors.uri?.message)}
                  {...register("uri", {
                    required:
                      "Нормативтік құжаттама веб-сайтына сілтеме обязателен",
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
              ЖАҢАРТУ
            </Button>
          </Box>
        </form>
      </div>
    </Modal>
  );
};

export default UpdateActs;
