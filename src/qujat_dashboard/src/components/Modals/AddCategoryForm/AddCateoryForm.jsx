import React from "react";
import { useForm, Controller } from "react-hook-form";
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
} from "@mui/material";

import {
  addNewCategory,
  fetchCategory,
} from "../../../redux/slices/categorySlice";

import close from "../../../assets/images/close.svg";
import "./AddCategoryForm.css";

const AddCateoryForm = ({ isOpen, handleClose, icons, setDataLength }) => {
  const dispatch = useDispatch();
  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({
    defaultValues: {
      nameKz: "",
      iconBlobId: 1,
      displayOrder: 0,
    },
    mode: "onChange",
  });

  const handleCloseModal = () => {
    handleClose();
    reset();
  };

  const onSubmit = async (data) => {
    const response = await dispatch(addNewCategory(data));

    if (response.responseData !== null) {
      handleCloseModal();
      dispatch(fetchCategory());
    }
  };

  // useEffect(() => {
  //   const getIcons = async () => {
  //     try {
  //       const response = await instance.get("/api/1/icons");

  //       if (response.status === 200) {
  //         setIcons(response.data.responseData);
  //       }
  //     } catch (error) {
  //       console.log(error);
  //     }
  //   };
  //   getIcons();
  // }, []);

  return (
    <Modal
      open={isOpen}
      onClose={handleClose}
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
          <h3>Қосу</h3>
          <img onClick={handleClose} src={close} className="close_icon" />
        </Box>
        <form className="add_category_form" onSubmit={handleSubmit(onSubmit)}>
          <Box sx={{ display: "flex", width: "100%", flexDirection: "column" }}>
            <Box sx={{ width: "100%", marginTop: 3 }}>
              <span className="main_text">Тақырыптық санат атауы</span>
              <TextField
                sx={{ marginTop: 1 }}
                fullWidth
                label="Тақырыптық санат атауын енгізіңіз"
                error={Boolean(errors.categoryName?.message)}
                {...register("nameKz", {
                  required: "Тақырыптық санат атауын енгізіңіз",
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
              <span className="main_text">Иконканы таңдаңыз</span>
              <FormControl sx={{ marginTop: 2 }}>
                <InputLabel id="select_icon">Иконка</InputLabel>
                <Controller
                  name="iconBlobId"
                  control={control}
                  render={({ field }) => (
                    <Select
                      {...field}
                      labelId="select_icon"
                      label="Иконка"
                      sx={{ width: "100%" }}
                      error={Boolean(errors.iconBlobId?.message)}
                    >
                      {icons?.map((item) => (
                        <MenuItem key={item.id} value={item.id}>
                          <img src={item.uri} />
                        </MenuItem>
                      ))}
                    </Select>
                  )}
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

export default AddCateoryForm;

const iconBlob = [
  {
    id: 1,
  },
  {
    id: 2,
  },
  {
    id: 3,
  },
];
