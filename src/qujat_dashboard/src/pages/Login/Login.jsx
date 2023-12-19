import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { Navigate } from "react-router-dom";

import {
  Container,
  Box,
  TextField,
  Button,
  IconButton,
  InputAdornment,
  styled,
} from "@mui/material";

import { FaEye } from "react-icons/fa";
import { IoEyeOff } from "react-icons/io5";
import logo from "../../assets/logo.png";

import { loginFetch } from "../../redux/slices/tokenSlice";
import { selectIsAuth } from "../../redux/slices/tokenSlice";

const StyledLogo = styled("img")({
  maxWidth: 160,
  marginBottom: 32,
  marginTop: 12,
});

const Login = () => {
  const [showPassword, setShowPassword] = useState(false);

  const handleTogglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const dispatch = useDispatch();
  const isAuth = useSelector(selectIsAuth);
  const { token } = useSelector((state) => state.token);
  const {
    register,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isValid },
  } = useForm({
    defaultValues: {
      email: "",
      password: "",
    },
    mode: "onChange",
  });

  const onSubmit = async (data) => {
    const response = await dispatch(loginFetch(data));
    reset();
    if ("accessToken" in response.payload) {
      window.localStorage.setItem("token", response.payload.accessToken);
    }
  };

  console.log("isAuth", isAuth);
  console.log("isAuth", token);

  if (isAuth) {
    return <Navigate to="/" />;
  }

  useEffect(() => {}, [token]);

  return (
    <Container maxWidth="xl">
      <Box
        sx={{
          width: "100%",
          height: "100vh",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
            width: "100%",
          }}
        >
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="modal_form_wrapper"
          >
            {/* LOGO */}
            <Box
              sx={{
                width: "100%",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              <StyledLogo src={logo} alt="Logo" />
            </Box>
            {/* INPUT EMAIL AND PASSWORD */}
            <Box
              sx={{
                width: "100%",
                display: "flex",
                flexDirection: "column",
                marginTop: 1,
              }}
            >
              <span className="main_text">
                Электрондық поштаңызды енгізіңіз
              </span>
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
            </Box>
            {/* LOGIN BUTTON */}
            <Button
              sx={{ marginTop: 4, marginBottom: 2 }}
              variant="contained"
              color="success"
              type="submit"
            >
              Кіру
            </Button>
          </form>
        </Box>
      </Box>
    </Container>
  );
};

export default Login;
