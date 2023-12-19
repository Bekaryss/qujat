import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";

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

// import { addBlob } from "../../../redux/slices/documentsSlice";

const AddPattern = ({ isOpenPattern, handleCloseCreate }) => {
  const dispatch = useDispatch();
  const [age, setAge] = useState("");
  const [isUpload, setIsUpload] = useState(false);

  const handleChange = (event) => {
    setAge(event.target.value);
  };

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isValid },
    reset,
  } = useForm({
    defaultValues: {
      categoryName: "",
      icon: "",
      file: null,
    },
    mode: "onBlur",
  });

  const onSubmit = async (data) => {
    const formData = new FormData();
    formData.append("rq", data.file[0]);

    const response = await dispatch(addNewDocument(formData));

    if (response.payload.requestSucceeded === true) {
      setIsUpload(true);
    }
  };

  useEffect(() => {}, []);

  return (
    <Modal
      open={isOpenPattern}
      onClose={handleCloseCreate}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
      className="modal_container"
    >
      <div className="modal_form_wrapper modal_pattern_form_wrapper">
        <Box
          sx={{
            width: "100%",
            display: "flex",
            align: "center",
            justifyContent: "space-between",
          }}
        >
          <h3>Жаңа құжат үлгісін қосу</h3>
          <img onClick={handleCloseCreate} src={close} className="close_icon" />
        </Box>
        <form className="add_category_form" onSubmit={handleSubmit(onSubmit)}>
          {!isUpload ? (
            <FormControl sx={{ marginTop: 1 }}>
              <Input
                type="file"
                inputProps={{
                  accept: ".pdf, .doc, .docx",
                }}
                {...register("file", {
                  required: "Выберите файл",
                })}
              />
              {errors.file && (
                <p style={{ color: "red" }}>{errors.file.message}</p>
              )}
            </FormControl>
          ) : (
            <Box
              sx={{ display: "flex", width: "100%", flexDirection: "column" }}
            >
              <Box sx={{ width: "100%", marginTop: 3 }}>
                <span className="main_text">
                  Құжат үлгісінің қазақша атын енгізіңіз
                </span>
                <TextField
                  size="small"
                  sx={{ marginTop: 1 }}
                  fullWidth
                  label="Құжат үлгісінің атын енгізіңіз"
                  error={Boolean(errors.categoryName?.message)}
                  {...register("categoryName", {
                    required: "Құжат үлгісінің атын енгізіңіз",
                  })}
                />
              </Box>
              <Box sx={{ width: "100%", marginTop: 1 }}>
                <span className="main_text">
                  Құжат үлгісінің орысша атын енгізіңіз
                </span>
                <TextField
                  size="small"
                  sx={{ marginTop: 1 }}
                  fullWidth
                  label="Құжат үлгісінің орысша атын енгізіңіз"
                  error={Boolean(errors.categoryName?.message)}
                  {...register("categoryName", {
                    required: "Құжат үлгісінің орысша атын енгізіңіз",
                  })}
                />
              </Box>
              <Box
                sx={{
                  width: "100%",
                  marginTop: 2,
                  display: "flex",
                  flexDirection: "column",
                }}
              >
                <span className="main_text">Құжат түрін таңдаңыз</span>
                <FormControl sx={{ marginTop: 1 }} size="small">
                  <InputLabel id="select_icon">Құжат түрін таңдаңыз</InputLabel>
                  <Select
                    labelId="select_icon"
                    value={age}
                    label="Құжат түрін таңдаңыз"
                    defaultValue={10}
                    onChange={handleChange}
                    sx={{ width: "100%" }}
                    error={Boolean(errors.icon?.message)}
                    {...register("icon", {
                      required: "Құжат түрін таңдаңыз",
                    })}
                  >
                    <MenuItem value="pdf">PDF</MenuItem>
                    <MenuItem value="word">Word</MenuItem>
                  </Select>
                </FormControl>
              </Box>
              <Box
                sx={{
                  width: "100%",
                  marginTop: 2,
                  display: "flex",
                  flexDirection: "column",
                }}
              >
                <span className="main_text">Құжат категориясының таңдаңыз</span>
                <FormControl sx={{ marginTop: 1 }} size="small">
                  <InputLabel id="select_icon">
                    Құжат категориясының таңдаңыз
                  </InputLabel>
                  <Select
                    labelId="select_icon"
                    value={age}
                    label="Иконка"
                    defaultValue={10}
                    onChange={handleChange}
                    sx={{ width: "100%" }}
                    error={Boolean(errors.icon?.message)}
                    {...register("icon", {
                      required: "Құжат категориясының таңдаңыз",
                    })}
                  >
                    <MenuItem value={10}>Documentation</MenuItem>
                  </Select>
                </FormControl>
              </Box>
              <Box
                sx={{
                  width: "100%",
                  marginTop: 2,
                  display: "flex",
                  flexDirection: "column",
                }}
              >
                <span className="main_text">
                  Құжат ішкі категориясының таңдаңыз
                </span>
                <FormControl sx={{ marginTop: 1 }} size="small">
                  <InputLabel id="select_icon">
                    Құжат ішкі категориясының таңдаңыз
                  </InputLabel>
                  <Select
                    labelId="select_icon"
                    value={age}
                    label="Иконка"
                    defaultValue={10}
                    onChange={handleChange}
                    sx={{ width: "100%" }}
                    error={Boolean(errors.icon?.message)}
                    {...register("icon", {
                      required: "Құжат категориясының таңдаңыз",
                    })}
                  >
                    <MenuItem value={10}>Documentation</MenuItem>
                  </Select>
                </FormControl>
              </Box>
            </Box>
          )}
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
              sx={{ width: "60%" }}
              type="submit"
            >
              ҚАБЫЛДАУ
            </Button>
            <Button
              color="error"
              variant="contained"
              sx={{ width: "40%", marginLeft: 1 }}
              onClick={handleCloseCreate}
            >
              ОТМЕНА
            </Button>
          </Box>
        </form>
      </div>
    </Modal>
  );
};

export default AddPattern;
