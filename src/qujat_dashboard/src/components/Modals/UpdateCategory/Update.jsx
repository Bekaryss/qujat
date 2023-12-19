import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector, useDispatch } from "react-redux";
import {
  Modal,
  Box,
  Stack,
  TextField,
  FormControl,
  MenuItem,
  InputLabel,
  Select,
  Button,
  Typography,
} from "@mui/material";

import close from "../../../assets/images/close.svg";
import instance from "../../../axios/axios";

import { fetchIcons } from "../../../redux/slices/IconSlice";
import { fetchCategory } from "../../../redux/slices/categorySlice";

const Update = ({
  updateModalOpen,
  handleUpdateCloseModal,
  id,
  dataLength,
}) => {
  const { icons, IsLoading } = useSelector((state) => state.icons);

  const [iconBlobId, setIconBlobId] = useState(
    icons?.length > 0 ? icons[0].id : ""
  );

  const dispatch = useDispatch();

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors, isValid },
    reset,
  } = useForm({
    defaultValues: {
      nameKz: "",
      iconBlobId: 0,
    },
    mode: "onBlur",
  });

  const updateCategory = async (categoryId, data) => {
    try {
      const response = await instance.patch(
        `/api/1/categories/${categoryId}`,
        data
      );

      if (response.status === 200) {
        dispatch(fetchCategory());
        handleUpdateCloseModal();
      }
    } catch (e) {
      console.log(e);
    }
  };

  const onSubmit = (values) => {
    console.log(values);
    updateCategory(id, values);
    reset();
  };

  useEffect(() => {
    const getCategoriesById = async (categoryId) => {
      try {
        const response = await instance.get(`/api/1/categories/${categoryId}`);
        if (response.status === 200) {
          setValue("nameKz", response.data.responseData.nameKz);
          if (IsLoading) {
            setIconBlobId(response.data.responseData.iconBlob.id);
          }
        }
      } catch (error) {
        console.log(error);
      }
    };

    if (id !== undefined && id !== 0) {
      getCategoriesById(id);
    }
    dispatch(fetchIcons());
  }, [dispatch, id, dataLength]);

  return (
    <Modal
      open={updateModalOpen}
      onClose={handleUpdateCloseModal}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
      className="modal_container"
    >
      <div className="modal_form_wrapper">
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
              onClick={handleUpdateCloseModal}
              src={close}
              className="close_icon"
            />
          </Box>

          <form className="add_category_form" onSubmit={handleSubmit(onSubmit)}>
            <Box
              sx={{ display: "flex", width: "100%", flexDirection: "column" }}
            >
              <Box sx={{ width: "100%", marginTop: 3 }}>
                <span className="main_text">Тақырыптық санат атауы</span>
                <TextField
                  sx={{ marginTop: 1 }}
                  fullWidth
                  error={Boolean(errors.nameKz?.message)}
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
                <span className="main_text">Иконканы тандаңыз</span>
                <FormControl sx={{ marginTop: 2 }}>
                  <InputLabel id="select_icon">Иконка</InputLabel>
                  <Select
                    labelId="select_icon"
                    label="Иконка"
                    value={iconBlobId} // Use the id instead of uri
                    sx={{ width: "100%" }}
                    error={Boolean(errors.iconBlobId?.message)}
                    {...register("iconBlobId", {
                      required: "Тақырыптын атын енгізіңіз",
                    })}
                  >
                    {icons?.map((item) => (
                      <MenuItem
                        key={item.id}
                        value={item.id} // Use the id as the value
                        onClick={() => {
                          setIconBlobId(item.id);
                        }}
                      >
                        <img src={item.uri} alt={item.nameKz} />
                      </MenuItem>
                    ))}
                  </Select>
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
                Жаңарту
              </Button>
            </Box>
          </form>
        </Box>
      </div>
    </Modal>
  );
};

export default Update;
