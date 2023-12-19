import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { Box, Modal, Button, Typography } from "@mui/material";
import close from "../../../assets/images/close.svg";
import {
  deleteCategory,
  fetchCategory,
} from "../../../redux/slices/categorySlice";

const Delete = ({
  deleteItem,
  handleCloseDeleteModal,
  id,
  dataLength,
  error,
}) => {
  const dispatch = useDispatch();

  const handleDelete = async (event) => {
    event.preventDefault(); // Предотвращение стандартного действия формы
    const response = await dispatch(deleteCategory(id));

    if (response.responseData !== null) {
      handleCloseDeleteModal();
      dispatch(fetchCategory());
    }
  };

  useEffect(() => {}, [dispatch, dataLength, error]);

  return (
    <Modal
      open={deleteItem}
      onClose={handleCloseDeleteModal}
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
            justifyContent: "space-between",
          }}
        >
          <Box
            sx={{
              width: "100%",
              display: "flex",
              align: "center",
              justifyContent: "space-between",
              cursor: "pointer",
            }}
          >
            <h3>Жоюды растаңыз</h3>
            <img
              onClick={handleCloseDeleteModal}
              src={close}
              className="close_icon"
            />
          </Box>
          <Box
            sx={{
              width: "100%",
              display: "flex",
              flexDirection: "column",
              paddingTop: 2,
              paddingBottom: 2,
            }}
          >
            <Box
              sx={{
                width: "100%",
                height: "1px",
                background: "#191919",
                marginBottom: 5,
              }}
            ></Box>
            <Typography variant="h6">
              Жойғыңыз келетініне сенімдісіз бе?. Бұл әрекетті кері қайтара
              алмайсыз.
            </Typography>
            <Box
              sx={{
                width: "100%",
                height: "1px",
                background: "#191919",
                marginTop: 5,
              }}
            ></Box>
            <Box sx={{ width: "100%", height: "1px" }}></Box>
          </Box>
          <form onSubmit={handleDelete}>
            <Box
              sx={{
                display: "flex",
                width: "100%",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              <Button
                variant="contained"
                color="primary"
                sx={{
                  width: "50%",
                  marginRight: 1,
                  background: "var(--main-color-500)",
                  "&:hover": {
                    backgroundColor: "var(--main-color-btn)",
                  },
                }}
                onClick={handleCloseDeleteModal}
              >
                Жоқ
              </Button>
              <Button
                color="error"
                sx={{
                  width: "50%",
                  marginLeft: 1,
                  background: "var(--main-red-500)",
                }}
                variant="contained"
                type="submit"
              >
                Иә
              </Button>
            </Box>
          </form>
        </Box>
      </div>
    </Modal>
  );
};

export default Delete;
