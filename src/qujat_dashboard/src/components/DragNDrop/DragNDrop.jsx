import React, { useState, useEffect, useRef } from "react";
import { Box, Button, CircularProgress, styled } from "@mui/material";
import { Link } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

import {
  fetchCategory,
  reorderCategories,
  saveReorderedCategories,
  setCategories,
} from "../../redux/slices/categorySlice";

import Update from "../Modals/UpdateCategory/Update";
import Delete from "../Modals/Delete/Delete";
import CantDeleteCategory from "../Alerts/CantDeleteCategory";
import OrderSaved from "../Alerts/OrderSaved";

import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";
import { MdEdit } from "react-icons/md";
import { MdOutlineDeleteSweep } from "react-icons/md";

const AddButton = styled(Button)({
  background: "var(--main-color-500)",
  color: "#fff",
  "&:hover": {
    backgroundColor: "var(--main-color-btn)",
  },

  "@media (max-width: 475px)": {
    width: "100%",
    marginTop: 8,
  },
});

const DragNDrop = ({ isOpen, handleClose, handleOpenModal }) => {
  const dispatch = useDispatch();
  const { categories, isLoading, error, reorderedCategoriesStatus } =
    useSelector((state) => state.categories);

  const [updateModalOpen, setUpdateModalOpen] = useState(false);
  const [modalDeleteAct, setModalDeleteAct] = useState(false);
  const [orderSaved, setOrderSaved] = useState(false);
  const [id, setId] = useState(0);

  // UPDATE MODAL HANDLER
  const handleUpdateOpenModal = (id) => {
    setUpdateModalOpen(true);
    setId(id);
  };

  const handleUpdateCloseModal = () => {
    setUpdateModalOpen(false);
  };

  // DELETE MODAL HANDLER
  const handleOpenDeleteModal = (id) => {
    setModalDeleteAct(true);
    setId(id);
  };

  const handleCloseDeleteModal = () => {
    setModalDeleteAct(false);
  };

  const onDragEnd = (result) => {
    if (!result.destination) return;

    const updatedCategories = Array.from(categories);
    const [reorderedItem] = updatedCategories.splice(result.source.index, 1);
    updatedCategories.splice(result.destination.index, 0, reorderedItem);

    // Update the displayOrder based on the new order
    const updatedCategoriesWithDisplayOrder = updatedCategories.map(
      (category, index) => ({
        ...category,
        displayOrder: index,
      })
    );

    // Dispatch the action only when drag is completed
    dispatch(reorderCategories(updatedCategoriesWithDisplayOrder));
    dispatch(setCategories(updatedCategoriesWithDisplayOrder));
  };

  const handleReorderSave = async () => {
    const response = await dispatch(
      saveReorderedCategories(categories.map((el) => el.id))
    );

    if (response.payload.requestSucceeded) {
      console.log(response.payload);
      setOrderSaved(true);
    }
  };

  useEffect(() => {
    dispatch(fetchCategory());
    // console.log(categories);

    // Reset orderSaved state after displaying the alert
    if (orderSaved) {
      setTimeout(() => {
        setOrderSaved(false);
      }, 3000); // Adjust the timeout as needed
    }
  }, [error, orderSaved]);

  return (
    <div style={{ width: "100%" }}>
      {error && <CantDeleteCategory />}
      {orderSaved && <OrderSaved />}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          marginTop: 2,
          marginBottom: 2,
        }}
      >
        <Button
          sx={{
            marginBottom: 1,
            marginTop: 1,
            background: "var(--main-color-500)",
            color: "#fff",
            "&:hover": {
              backgroundColor: "var(--main-color-btn)",
            },
          }}
          onClick={handleReorderSave}
          variant="outlined"
        >
          Сақтау
        </Button>

        <AddButton onClick={handleOpenModal}>Қосу</AddButton>
      </Box>

      <DragDropContext onDragEnd={onDragEnd}>
        <Droppable droppableId="tasks" direction="vertical">
          {(provided) => (
            <Box
              sx={{
                display: "flex",
                width: "100%",
                flexWrap: "wrap",
                gap: "10px",
              }}
              {...provided.droppableProps}
              ref={provided.innerRef}
            >
              {isLoading ? (
                <Box
                  sx={{
                    display: "flex",
                    width: "100%",
                    alignItems: "center",
                    justifyContent: "center",
                    position: "absolute",
                    left: "2%",
                    top: "45%",
                  }}
                >
                  <CircularProgress size={45} />
                </Box>
              ) : (
                categories.map((item, index) => (
                  <Draggable
                    key={item.displayOrder}
                    draggableId={item.displayOrder.toString()}
                    index={index}
                  >
                    {(provided) => (
                      <Box
                        ref={provided.innerRef}
                        {...provided.draggableProps}
                        {...provided.dragHandleProps}
                        sx={{
                          display: "flex",
                          alignItems: "center",
                          justifyContent: "space-between",
                          height: "80px",
                          width: "100%",
                          background: "#fff",
                          padding: "24px",
                          borderRadius: "10px",
                        }}
                      >
                        <Box sx={{ display: "flex", alignItems: "center" }}>
                          <Box sx={{ marginRight: 2 }}>
                            <img src={item?.iconBlob?.uri} alt="" />
                          </Box>
                          <h2>{item.nameKz}</h2>
                        </Box>
                        <Box sx={{ display: "flex", alignItems: "center" }}>
                          <Box
                            sx={{
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              border: "1px solid var(--black)",
                              borderRadius: "8px",
                              padding: 0.5,
                              cursor: "pointer",
                              marginRight: 2,
                              transition: "var(--tran-bg-03)",
                              "&:hover": {
                                background: "var(--main-color-btn)",
                              },
                            }}
                            onClick={() => {
                              handleUpdateOpenModal(item.id);
                            }}
                          >
                            <MdEdit style={{ fontSize: "20px" }} />
                          </Box>
                          <Box
                            sx={{
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              border: "1px solid var(--black)",
                              borderRadius: "8px",
                              padding: 0.5,
                              cursor: "pointer",
                              marginRight: 2,
                              transition: "var(--tran-bg-03)",
                              "&:hover": {
                                background: "var(--main-color-btn)",
                              },
                            }}
                            onClick={() => {
                              handleOpenDeleteModal(item.id);
                            }}
                          >
                            <MdOutlineDeleteSweep
                              style={{
                                fontSize: "20px",
                                color: "var(--main-red)",
                              }}
                            />
                          </Box>
                          <Link
                            to={`/subcategories/${item.id}`}
                            style={{
                              textDecoration: "none",
                              color: "var(--black)",
                              "&:hover": {
                                color: "var(--black)",
                              },
                            }}
                          >
                            <Button
                              sx={{
                                marginLeft: 3,
                                transition: "var(--tran-bg-03)",
                                "&:hover": {
                                  background: "var(--main-color-btn)",
                                  color: "var(--black)",
                                },
                              }}
                              variant="outlined"
                            >
                              толығырақ
                            </Button>
                          </Link>
                        </Box>
                      </Box>
                    )}
                  </Draggable>
                ))
              )}
              {provided.placeholder}
            </Box>
          )}
        </Droppable>
      </DragDropContext>
      <Update
        updateModalOpen={updateModalOpen}
        handleUpdateCloseModal={handleUpdateCloseModal}
        id={id}
      />
      <Delete
        deleteItem={modalDeleteAct}
        handleCloseDeleteModal={handleCloseDeleteModal}
        id={id}
        error={error}
      />
    </div>
  );
};

export default DragNDrop;
