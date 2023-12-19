import React, { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { useDispatch } from "react-redux";
import {
  Box,
  TextField,
  Modal,
  Button,
  InputAdornment,
  IconButton,
  Typography,
} from "@mui/material";

import { FaEye } from "react-icons/fa";
import { IoEyeOff } from "react-icons/io5";
import close from "../../../assets/images/close.svg";
import instance from "../../../axios/axios";

const AddAdmin = ({ isOpenAddAdmin, handleCloseAdd }) => {
  const dispatch = useDispatch();
  const [showPassword, setShowPassword] = useState(false);

  const handleTogglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({
    defaultValues: {
      email: "",
      password: "",
    },
    mode: "onChange",
  });

  const addAdmin = async (data) => {
    try {
      const response = await instance.post("/api/1/application-users", data);

      if (response.status === 200) {
        handleCloseModal();
      }
    } catch (err) {
      throw err;
    }
  };

  const handleCloseModal = () => {
    handleCloseAdd();
    reset();
  };

  const onSubmit = async (data) => {
    console.log(data);
    addAdmin(data);
  };

  return (
    <Modal
      open={isOpenAddAdmin}
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
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          <h3>Жаңа админ қосу</h3>
          <img onClick={handleCloseModal} src={close} className="close_icon" />
        </Box>
        <form className="add_category_form" onSubmit={handleSubmit(onSubmit)}>
          <Box
            sx={{
              width: "100%",
              display: "flex",
              flexDirection: "column",
              marginTop: 1,
            }}
          >
            <span className="main_text">Электрондық поштаңызды енгізіңіз</span>
            {/* EMAIL */}
            <TextField
              sx={{ marginTop: 2 }}
              label="Электрондық поштаңызды енгізіңіз"
              type="email"
              error={Boolean(errors.email?.message)}
              helperText={errors.email?.message}
              {...register("email", {
                required: "Электрондық поштаңызды енгізіңіз",
              })}
            />
          </Box>
          <Box
            sx={{
              width: "100%",
              display: "flex",
              flexDirection: "column",
              marginTop: 1,
            }}
          >
            <span className="main_text">Құпия сөз</span>
            {/* PASSWORD */}
            <TextField
              type={showPassword ? "text" : "password"}
              label="Құпия сөзді еңгізңіз"
              sx={{ marginTop: 2 }}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={handleTogglePasswordVisibility}>
                      {showPassword ? <FaEye /> : <IoEyeOff />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
              error={Boolean(errors.password?.message)}
              helperText={errors.password?.message}
              {...register("password", {
                required: "Құпия сөзді еңгізңіз",
              })}
            />
            <Typography
              variant="span"
              sx={{ fontSize: "14px", fontStyle: "italic", marginTop: 1 }}
            >
              Құпия сөз талаптары 8 таңбадан тұрады, бір бас әріп, бір кіші әріп
              және сан
            </Typography>
          </Box>
          <Box sx={{ width: "100%", marginTop: 3 }}>
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
          </Box>
        </form>
      </div>
    </Modal>
  );
};

export default AddAdmin;
