import React, { useEffect } from "react";
import { useParams } from "react-router-dom";
import { Box, Modal, Button, Typography } from "@mui/material";
import close from "../../../assets/images/close.svg";
import instance from "../../../axios/axios";

const DeleteSubCategory = ({
  isOpenDelete,
  handleCloseDelete,
  subCategoryId,
  setDataLength,
}) => {
  const { categoryId } = useParams();

  const handleDelete = async () => {
    try {
      const response = await instance.delete(
        `/api/1/categories/${categoryId}/subcategories/${subCategoryId}`,
        {
          body: JSON.stringify({}),
        }
      );

      if (response.status === 200) {
        handleCloseDelete();
        alert("SubCategories deleted");
        setDataLength((prev) => prev + 1);
      }
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {}, [subCategoryId]);

  return (
    <Modal
      open={isOpenDelete}
      onClose={handleCloseDelete}
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
              onClick={handleCloseDelete}
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
            <Typography variant="body1">
              Ішкі санатты біржола жойғыңыз келетініне сенімдісіз бе. Бұл
              әрекетті кері қайтара алмайсыз.
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
              onClick={handleCloseDelete}
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
              onClick={handleDelete}
            >
              Иә
            </Button>
          </Box>
        </Box>
      </div>
    </Modal>
  );
};

export default DeleteSubCategory;
